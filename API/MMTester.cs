namespace MurderMystery.API
{
    public class MMTester
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
                        Log.Error($"(MMTester) Rotation was not found! {Mathf.RoundToInt(HczArmory.gameObject.transform.rotation.eulerAngles.y)}, the tester will not work!");
                        Singleton = null;
                        break;
                }

                TesterCooldown = -1f;

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

        public Vector2 X { get; }
        public Vector2 Y { get; }
        public Vector2 Z { get; }

        public float TesterCooldown { get; private set; }

        public bool WithinRange(Vector3 pos)
        {
            return (pos.x >= X.x) && (pos.x <= X.y) && (pos.y >= Y.x) && (pos.y <= Y.y) && (pos.z >= Z.x) && (pos.z <= Z.y);
        }

        internal IEnumerator<float> TesterCoroutine()
        {
            List<MMPlayer> Players = new List<MMPlayer>();

            while (true)
            {
                yield return Timing.WaitForSeconds(0.1f);

                if (TesterCooldown <= 0f)
                {
                    Players.Clear();

                    for (int i = 0; i < MMPlayer.List.Count; i++)
                    {
                        if (MMPlayer.List[i].IsAlive())
                        {
                            Players.Add(MMPlayer.List[i]);
                        }
                    }

                    if (Players.Count != 3)
                    {
                        continue;
                    }


                }
                else
                {
                    while (TesterCooldown > 0f)
                    {
                        Timing.WaitForSeconds(1f);
                        TesterCooldown -= 1f
                    }
                }
            }
        }
    }
}