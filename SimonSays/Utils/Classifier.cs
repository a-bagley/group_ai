using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimonSays.Utils
{
    interface Classifier
    {
        void trainAI(RawSkeletalDataPackage skeletalDataPackage);
        Guess makeGuess(SkeletonDataRow skelDataRow);

    }
}
