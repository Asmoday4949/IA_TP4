using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.ComponentModel;

namespace OthelloIAFH
{
    public enum Player { White = 0, Black = 1 };

    [Serializable]
    public class PlayerData : INotifyPropertyChanged
    {
        private int numberOfPawns = 2;
        private int secondsElapsed = 0;
        private bool hasSkippedLastTurn = false;

        [field: NonSerializedAttribute()]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public PlayerData()
        {
        }

        /// <summary>
        /// Clear the score the pawn. Prefer this method for clearing score. This way, you avoid the binding update.
        /// </summary>
        public void ClearScore()
        {
            this.numberOfPawns = 0;
        }

        /// <summary>
        /// Get or set the number of pawns. When set, the value is updated with the graphical interface
        /// </summary>
        public int NumberOfPawns
        {
            get { return numberOfPawns; }
            set
            {
                numberOfPawns = value;
                RaisePropertyChanged("NumberOfPawns");
            }
        }

        /// <summary>
        /// Get and set seconds elapsed. When set, the value is updated with the graphical interface
        /// </summary>
        public int SecondsElapsed
        {
            get { return this.secondsElapsed; }
            set
            {
                secondsElapsed = value;
                RaisePropertyChanged("SecondsElapsed");
            }
        }

        /// <summary>
        /// Inform that a property changed. Used for binding.
        /// </summary>
        /// <param name="propertyName">Property which has changed</param>
        public void RaisePropertyChanged(String propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// Whether this player has skipped his turn.
        /// </summary>
        public bool HasSkippedLastTurn
        {
            get { return hasSkippedLastTurn;  }
            set { hasSkippedLastTurn = value; }
        }
    }
}
