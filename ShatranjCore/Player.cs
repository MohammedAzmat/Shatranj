using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShatranjCore
{
    public enum PlayerType { Human, AI}
    public class Player
    {
        private PieceColor playerColor;
        private MoveMaker playerMove;
        private PlayerType playerType;
        private bool playerTurn;

        public PieceColor Color
        {
            get
            {
                return playerColor;
            }
            set
            {
                this.playerColor = value;
            }
        }

        public PlayerType Type
        {
            get { return playerType; }
            set { this.playerType = value; }
        }

        public bool HasTurn
        {
            get { return playerTurn; }
            set { this.playerTurn = value; }
        }

        /// <summary>
        /// Constructor for Player, Accepts Piece Color and PlayerType
        /// </summary>
        /// <param name="pc">Color of Player, Can be White or Black</param>
        /// <param name="pType">Defines the Type of Player, Human or AI</param>
        public Player(PieceColor pc, PlayerType pType = PlayerType.Human)
        {
            playerType = pType;
            playerColor = pc;
            playerTurn = false;
        }
    }
}
