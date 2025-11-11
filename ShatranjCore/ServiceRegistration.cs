using System;
using System.Collections.Generic;
using ShatranjCore.Abstractions;
using ShatranjCore.Abstractions.Interfaces;
using ShatranjCore.Application;
using ShatranjCore.Application.CommandHandlers;
using ShatranjCore.Board;
using ShatranjCore.Domain;
using ShatranjCore.Interfaces;
using ShatranjCore.Learning;
using ShatranjCore.Logging;
using ShatranjCore.Movement;
using ShatranjCore.Persistence;
using ShatranjCore.Persistence.Exporters;
using ShatranjCore.Settings;
using ShatranjCore.State;
using ShatranjCore.UI;
using ShatranjCore.Validators;

namespace ShatranjCore
{
    /// <summary>
    /// Service Registration - Central place for Dependency Injection setup.
    /// Follows the Dependency Inversion Principle: Depend on abstractions, not concrete classes.
    ///
    /// Usage:
    ///   var services = ServiceRegistration.RegisterCoreServices();
    ///   var gameFactory = services.GetService(typeof(IGameFactory)) as IGameFactory;
    /// </summary>
    public class ServiceRegistration
    {
        /// <summary>
        /// Register all core game services in a simple dictionary-based container.
        /// In a production system, you would use Microsoft.Extensions.DependencyInjection.
        /// </summary>
        public static IServiceProvider RegisterCoreServices(ILogger logger = null)
        {
            var container = new ServiceContainer();

            // Logger (required first)
            if (logger == null)
            {
                logger = new CompositeLogger(
                    new FileLogger(),
                    new ConsoleLogger(includeTimestamp: false)
                );
            }
            container.Register(typeof(ILogger), logger);

            // ===== BOARD & STATE =====
            container.Register(typeof(IChessBoard), typeof(ChessBoard));
            container.Register(typeof(MoveHistory), typeof(MoveHistory));

            // ===== VALIDATORS =====
            container.Register(typeof(CheckDetector), typeof(CheckDetector));
            container.Register(typeof(CastlingValidator), typeof(CastlingValidator));
            container.Register(typeof(EnPassantTracker), typeof(EnPassantTracker));

            // ===== DOMAIN LAYER =====
            container.Register(typeof(MoveExecutor), typeof(MoveExecutor));
            container.Register(typeof(TurnManager), typeof(TurnManager));
            container.Register(typeof(CastlingExecutor), typeof(CastlingExecutor));
            container.Register(typeof(PromotionRule), typeof(PromotionRule));

            // ===== STATE MANAGEMENT =====
            container.Register(typeof(GameStateManager), typeof(GameStateManager));
            container.Register(typeof(SnapshotManager), typeof(SnapshotManager));

            // ===== PERSISTENCE =====
            container.Register(typeof(SaveGameManager), typeof(SaveGameManager));
            container.Register(typeof(GameSerializer), typeof(GameSerializer));
            container.Register(typeof(PieceFactory), typeof(PieceFactory));
            container.Register(typeof(GameConfigManager), typeof(GameConfigManager));

            // ===== EXPORTERS =====
            container.Register(typeof(IPGNExporter), typeof(PGNExporter));
            container.Register(typeof(IFENExporter), typeof(FENExporter));

            // ===== UI LAYER =====
            container.Register(typeof(ConsoleBoardRenderer), typeof(ConsoleBoardRenderer));
            container.Register(typeof(CommandParser), typeof(CommandParser));
            container.Register(typeof(ConsolePromotionUI), typeof(ConsolePromotionUI));
            container.Register(typeof(ConsoleMoveHistoryRenderer), typeof(ConsoleMoveHistoryRenderer));
            container.Register(typeof(GameMenuHandler), typeof(GameMenuHandler));
            container.Register(typeof(SettingsManager), typeof(SettingsManager));

            // ===== LEARNING & RECORDING =====
            container.Register(typeof(GameRecorder), typeof(GameRecorder));

            // ===== APPLICATION LAYER =====
            container.Register(typeof(CommandProcessor), typeof(CommandProcessor));
            container.Register(typeof(CommandHandlerFactory), typeof(CommandHandlerFactory));
            container.Register(typeof(GameLoop), typeof(GameLoop));
            container.Register(typeof(GameOrchestrator), typeof(GameOrchestrator));

            return container;
        }

        /// <summary>
        /// Register all services including AI.
        /// </summary>
        public static IServiceProvider RegisterWithAI(IChessAI whiteAI, IChessAI blackAI, ILogger logger = null)
        {
            var container = RegisterCoreServices(logger) as ServiceContainer;

            // Register AI instances
            container.Register("WhiteAI", whiteAI);
            container.Register("BlackAI", blackAI);

            return container;
        }
    }

    /// <summary>
    /// Simple Service Container implementation.
    /// Implements IServiceProvider for basic service resolution.
    /// In production, use Microsoft.Extensions.DependencyInjection instead.
    /// </summary>
    public interface IServiceProvider
    {
        object GetService(Type serviceType);
        T GetService<T>() where T : class;
    }

    /// <summary>
    /// Dictionary-based service container for dependency injection.
    /// </summary>
    public class ServiceContainer : IServiceProvider
    {
        private readonly Dictionary<Type, object> _singletons = new Dictionary<Type, object>();
        private readonly Dictionary<Type, Type> _typeRegistrations = new Dictionary<Type, Type>();
        private readonly Dictionary<string, object> _namedRegistrations = new Dictionary<string, object>();
        private readonly ILogger _logger;

        public ServiceContainer(ILogger logger = null)
        {
            _logger = logger;
        }

        /// <summary>
        /// Register a singleton instance.
        /// </summary>
        public void Register(Type serviceType, object instance)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            _singletons[serviceType] = instance;
            _logger?.Debug($"Registered singleton: {serviceType.Name} = {instance.GetType().Name}");
        }

        /// <summary>
        /// Register a type mapping (interface -> implementation).
        /// </summary>
        public void Register(Type serviceType, Type implementationType)
        {
            if (serviceType == null) throw new ArgumentNullException(nameof(serviceType));
            if (implementationType == null) throw new ArgumentNullException(nameof(implementationType));

            _typeRegistrations[serviceType] = implementationType;
            _logger?.Debug($"Registered type: {serviceType.Name} -> {implementationType.Name}");
        }

        /// <summary>
        /// Register a named instance (for disambiguation).
        /// </summary>
        public void Register(string name, object instance)
        {
            if (string.IsNullOrEmpty(name)) throw new ArgumentNullException(nameof(name));
            if (instance == null) throw new ArgumentNullException(nameof(instance));

            _namedRegistrations[name] = instance;
            _logger?.Debug($"Registered named singleton: {name} = {instance.GetType().Name}");
        }

        /// <summary>
        /// Get a service instance.
        /// </summary>
        public object GetService(Type serviceType)
        {
            if (serviceType == null)
                throw new ArgumentNullException(nameof(serviceType));

            // Check singleton registrations first
            if (_singletons.TryGetValue(serviceType, out var singleton))
            {
                return singleton;
            }

            // Check type registrations and create instance
            if (_typeRegistrations.TryGetValue(serviceType, out var implementationType))
            {
                try
                {
                    var instance = Activator.CreateInstance(implementationType);
                    // Cache as singleton
                    _singletons[serviceType] = instance;
                    return instance;
                }
                catch (Exception ex)
                {
                    _logger?.Error($"Failed to create instance of {implementationType.Name}", ex);
                    throw new InvalidOperationException(
                        $"Could not resolve service {serviceType.Name} -> {implementationType.Name}",
                        ex);
                }
            }

            throw new InvalidOperationException(
                $"Service {serviceType.Name} is not registered in the container");
        }

        /// <summary>
        /// Generic version of GetService.
        /// </summary>
        public T GetService<T>() where T : class
        {
            return GetService(typeof(T)) as T;
        }

        /// <summary>
        /// Get a named service.
        /// </summary>
        public object GetNamedService(string name)
        {
            if (_namedRegistrations.TryGetValue(name, out var instance))
            {
                return instance;
            }

            throw new InvalidOperationException($"Named service '{name}' is not registered");
        }

        /// <summary>
        /// Get a named service with generic type.
        /// </summary>
        public T GetNamedService<T>(string name) where T : class
        {
            return GetNamedService(name) as T;
        }
    }
}
