namespace MurderMystery.API
{
    /*public class MMTester
    {
        internal MMTester()
        {
            HczArmory = Map.GetDoorByName("HCZ_ARMORY");

            if (HczArmory != null)
            {
                switch (Mathf.RoundToInt(HczArmory.gameObject.transform.rotation.eulerAngles.y))
                {
                    case 0:
                        X = new Vector2(HczArmory.gameObject.transform.position.x - 3.1f, HczArmory.gameObject.transform.position.x + 3.1f);
                        Y = new Vector2(HczArmory.gameObject.transform.position.y, HczArmory.gameObject.transform.position.y + 3.5f);
                        Z = new Vector2(HczArmory.gameObject.transform.position.z - 6.15f, HczArmory.gameObject.transform.position.z);
                        break;
                    case 90:
                        X = new Vector2(HczArmory.gameObject.transform.position.x - 6.15f, HczArmory.gameObject.transform.position.x);
                        Y = new Vector2(HczArmory.gameObject.transform.position.y, HczArmory.gameObject.transform.position.y + 3.5f);
                        Z = new Vector2(HczArmory.gameObject.transform.position.z - 3.1f, HczArmory.gameObject.transform.position.z + 3.1f);
                        break;
                    case 180:
                        X = new Vector2(HczArmory.gameObject.transform.position.x - 3.1f, HczArmory.gameObject.transform.position.x + 3.1f);
                        Y = new Vector2(HczArmory.gameObject.transform.position.y, HczArmory.gameObject.transform.position.y + 3.5f);
                        Z = new Vector2(HczArmory.gameObject.transform.position.z, HczArmory.gameObject.transform.position.z + 6.15f);
                        break;
                    case 270:
                        X = new Vector2(HczArmory.gameObject.transform.position.x, HczArmory.gameObject.transform.position.x + 6.15f);
                        Y = new Vector2(HczArmory.gameObject.transform.position.y, HczArmory.gameObject.transform.position.y + 3.5f);
                        Z = new Vector2(HczArmory.gameObject.transform.position.z - 3.1f, HczArmory.gameObject.transform.position.z + 3.1f);
                        break;
                    default:
                        Log.Error($"(MMTester ctor) Rotation was not found! {Mathf.RoundToInt(HczArmory.gameObject.transform.rotation.eulerAngles.y)}, the tester will not work!");
                        Singleton = null;
                        break;
                }

                Singleton = this;
            }
            else
            {
                Log.Error("(MMTester) Major error! HczArmory was null, the tester will not work!");

                Singleton = null;
            }
        }

        public static MMTester Singleton { get; internal set; }

        public DoorVariant HczArmory { get; }

        /// <summary>
        /// Represents the minimum (x) and maximum (y) float values for x position to be within the x range of the tester room.
        /// </summary>
        public Vector2 X { get; }
        /// <summary>
        /// Represents the minimum (x) and maximum (y) float values for y position to be within the y range of the tester room.
        /// </summary>
        public Vector2 Y { get; }
        /// <summary>
        /// Represents the minimum (x) and maximum (y) float values for z position to be within the z range of the tester room.
        /// </summary>
        public Vector2 Z { get; }

        public bool WithinRange(Vector3 pos)
        {
            return (pos.x >= X.x) && (pos.x <= X.y) && (pos.y >= Y.x) && (pos.y <= Y.y) && (pos.z >= Z.x) && (pos.z <= Z.y);
        }
    }*/
}