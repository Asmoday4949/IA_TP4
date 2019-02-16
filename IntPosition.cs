using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OthelloIAFH
{
    [Serializable]
    public struct IntPosition
    {
        private int row;
        private int column;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="column">Value for column</param>
        /// <param name="row">Value for row</param>
        public IntPosition(int column = -1, int row = -1)
        {
            this.column = column;
            this.row = row;
        }

        /// <summary>
        /// Give a string representation of this object
        /// </summary>
        /// <returns>String representation of this object</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(this.GetType().Name);
            builder.Append(":");
            builder.Append(column);
            builder.Append(":");
            builder.Append(row);
            return builder.ToString();
        }

        /// <summary>
        /// Addition
        /// </summary>
        /// <param name="intA"></param>
        /// <param name="intB"></param>
        /// <returns></returns>
        public static IntPosition operator+(IntPosition intA, IntPosition intB)
        {
            return new IntPosition(intA.Column + intB.Column, intA.Row + intB.Row);
        }

        /// <summary>
        /// Subtraction
        /// </summary>
        /// <param name="intA"></param>
        /// <param name="intB"></param>
        /// <returns></returns>
        public static IntPosition operator-(IntPosition intA, IntPosition intB)
        {
            return new IntPosition(intA.Column - intB.Column, intA.Row - intB.Row);
        }

        /// <summary>
        /// Get number of rows
        /// </summary>
        public int Row
        {
            get { return this.row; }
        }

        /// <summary>
        /// Get number of columns
        /// </summary>
        public int Column
        {
            get { return this.column; }
        }
    }
}
