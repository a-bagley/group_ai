using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NeuralNetwork
{
    class Guess
    {
        private int guessId {get; set; }

        private double guessValue { get; set; }

        public Guess() {

        }

        public Guess(int id, double value) {
            this.guessId = id;
            this.guessValue = value;
        }
    }
}
