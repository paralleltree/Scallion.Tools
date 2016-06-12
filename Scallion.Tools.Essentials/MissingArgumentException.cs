using System;
using System.Collections.Generic;
using System.Linq;

namespace Scallion.Tools.Essentials
{
    /// <summary>
    /// Represents an exception that is thrown when the arguments is not enough to process.
    /// </summary>
    public class MissingArgumentException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="MissingArgumentException"/> class.
        /// </summary>
        public MissingArgumentException()
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingArgumentException"/> class with a specified message.
        /// </summary>
        /// <param name="message">A description of the error.</param>
        public MissingArgumentException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="MissingArgumentException"/> class
        /// with a specified message and a reference to the inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="inner"></param>
        public MissingArgumentException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
