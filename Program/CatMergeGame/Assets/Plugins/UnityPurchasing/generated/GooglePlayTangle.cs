#if UNITY_ANDROID || UNITY_IPHONE || UNITY_STANDALONE_OSX || UNITY_TVOS
// WARNING: Do not modify! Generated file.

namespace UnityEngine.Purchasing.Security {
    public class GooglePlayTangle
    {
        private static byte[] data = System.Convert.FromBase64String("8ApXnRwdnnhCfZWkpfyQi9eLCZAc2DR6/ma3CmJ5Vxqneqxn8kDfz7mkx75+acdpMVLSja1WKbpQb4ZN5hXf+GU0zMIPn626+dqGMGEo2Z+3umeGhPw75lyf5AyviI/pFgtogmqJIOlGHnJ5sdK+qj+r4RBs/p9ogVbinQp4PaHraaoQZqp+1AM4fS/RY+DD0ezn6MtnqWcW7ODg4OTh4qwEa30dh3VDFDEOyQmEnWXixKcKY+Du4dFj4OvjY+Dg4WU0NqNP8l8WdUkUW5DXDsTl+bnHmoKX/lVvb9b1T1p96/w9PrYsi+zISake79dJkdfoG/3MtGdmE0PVVV//9RZvwEhpEXx+1Grf/im4+660tlgzYNI95XP8ixfcphpomuPi4OHg");
        private static int[] order = new int[] { 8,9,2,3,7,7,13,8,12,9,13,12,13,13,14 };
        private static int key = 225;

        public static readonly bool IsPopulated = true;

        public static byte[] Data() {
        	if (IsPopulated == false)
        		return null;
            return Obfuscator.DeObfuscate(data, order, key);
        }
    }
}
#endif
