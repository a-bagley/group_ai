using System;
using System.Collections.Generic;

namespace SimonSays.Utils
{
    /// <summary>
    /// Storage class for all the data required by AI systems.
    /// </summary>
    class RawSkeletalDataPackage
    {
        /// <summary>
        /// All the Skeletal training data organised by gesture keys
        /// </summary>
        private Dictionary<String, List<SkeletonDataRow>> mDataDic;

        /// <summary>
        /// List of gesture names
        /// </summary>
        private List<String> mGestureNameList;

        /// <summary>
        /// Total number of gestures
        /// </summary>
        private int mTotalGestures;

        /// <summary>
        /// Total number of skeletal traing data rows
        /// </summary>
        private int mTotalDataRows;

        public RawSkeletalDataPackage (Dictionary<String, List<SkeletonDataRow>> dd, List<String> gestureNames, int numberGestures)
        {
            mDataDic = dd;
            mGestureNameList = gestureNames;
            mTotalDataRows = numberGestures;
            mTotalGestures = gestureNames.Count;
        }

        public Dictionary<String, List<SkeletonDataRow>> getSkeletalDataDic()
        {
            return mDataDic;
        }

        public List<String> getGestureNameList()
        {
            return mGestureNameList;
        }

        public int getTotalDataRows()
        {
            return mTotalDataRows;
        }

        public int getTotalGestures()
        {
            return mTotalGestures;
        }
    }
}
