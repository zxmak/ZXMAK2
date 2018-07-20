using ZXMAK2.Engine;
using ZXMAK2.Serializers.SnapshotSerializers;


namespace ZXMAK2.Serializers
{
	public class LoadManager : SerializeManager
	{
		private readonly Spectrum _spec;

        public LoadManager(Spectrum spec)
        {
            _spec = spec;
            Clear();
        }

        public override void Clear()
        {
            base.Clear();
            // Default Serializers (Snapshots)...
            AddSerializer(new SzxSerializer(_spec));
            AddSerializer(new Z80Serializer(_spec));
            AddSerializer(new SnaSerializer(_spec));
            AddSerializer(new SitSerializer(_spec));
            AddSerializer(new ZxSerializer(_spec));
            AddSerializer(new RzxSerializer(_spec));
        }
	}
}
