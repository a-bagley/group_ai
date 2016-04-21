using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimonSays.Utils
{
    class RawSkeletalDataPackage
    {
        private Dictionary<String, List<SkeletonDataRow>> mDataDic;

        private List<String> mGestureNameList;

        private int mTotalGestures;

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
