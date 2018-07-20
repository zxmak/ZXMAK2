using System;
using System.IO;
using System.Collections.Generic;
using ZXMAK2.Model.Disk;


namespace ZXMAK2.Serializers.DiskSerializers
{
    public abstract class SectorImageSerializerBase : FormatSerializer
    {
        #region Fields

        private DiskImage m_diskImage;

        #endregion Fields


        #region Properties

        public override string FormatName
        {
            get { return string.Format("{0} disk image", FormatExtension); }
        }

        public override string FormatGroup
        {
            get { return "Disk images"; }
        }

        public override bool CanDeserialize
        {
            get { return true; }
        }

        public override bool CanSerialize
        {
            get { return true; }
        }

        #endregion

        #region Public

        public SectorImageSerializerBase(DiskImage diskImage)
        {
            m_diskImage = diskImage;
        }

        public override void Deserialize(Stream stream)
        {
            LoadFromStream(stream);
            m_diskImage.ModifyFlag = ModifyFlag.None;
            m_diskImage.Present = true;
        }

        public override void Serialize(Stream stream)
        {
            SaveToStream(stream);
            m_diskImage.ModifyFlag = ModifyFlag.None;
        }

        public override void SetReadOnly(bool readOnly)
        {
            m_diskImage.IsWP = readOnly;
        }

        public override void SetSource(string fileName)
        {
            m_diskImage.FileName = fileName;
        }

        #endregion Public


        #region Abstract

        protected abstract int SectorSizeCode { get; }

        protected abstract int[] GetSectorMap(int cyl, int head);

        #endregion Abstract


        #region Private

        protected virtual void LoadFromStream(Stream stream)
        {
            var secSize = 128 << (SectorSizeCode & 3);
            var cylSize = GetSectorMap(0,0).Length * secSize * 2;
            var cylCount = (int)(stream.Length / cylSize);
            if ((stream.Length % cylSize) > 0L)
            {
                cylCount += 1;
            }
            m_diskImage.SetPhysics(cylCount, 2);

            for (var c = 0; c < m_diskImage.CylynderCount; c++)
            {
                for (var h = 0; h < m_diskImage.SideCount; h++)
                {
                    var sectorList = new List<Sector>();
                    var il = GetSectorMap(c, h);
                    for (var s = 0; s < il.Length; s++)
                    {
                        var sector = new SimpleSector(
                            c,
                            h,
                            il[s],
                            SectorSizeCode & 3,
                            new byte[secSize]);
                        stream.Read(sector.Data, 0, sector.Data.Length);
                        sector.SetAdCrc(true);
                        sector.SetDataCrc(true);
                        sectorList.Add(sector);
                    }
                    m_diskImage
                        .GetTrackImage(c, h)
                        .AssignSectors(sectorList);
                }
            }
            m_diskImage.ModifyFlag = ModifyFlag.None;
        }

        protected virtual void SaveToStream(Stream stream)
        {
            // save at least 80 cylinders
            var cylCount = m_diskImage.CylynderCount < 80 ?
                80 : m_diskImage.CylynderCount;
            var secSize = 128 << (SectorSizeCode & 3);
            for (var c = 0; c < cylCount; c++)
            {
                for (var h = 0; h < 2; h++)
                {
                    var il = GetSectorMap(c, h);
                    for (var s = 0; s < il.Length; s++)
                    {
                        var buffer = new byte[secSize];
                        if (c < m_diskImage.CylynderCount)
                        {
                            if (m_diskImage.GetLogicalSectorSizeCode(c, h, il[s]) != (SectorSizeCode & 3))
                            {
                                throw new NotSupportedException(
                                    string.Format(
                                        "Cannot save because disk contains data which is not supported by {0}",
                                        FormatName));
                            }
                            m_diskImage
                                .ReadLogicalSector(c, h, il[s], buffer);
                        }
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            }
        }

        #endregion Private
    }
}
