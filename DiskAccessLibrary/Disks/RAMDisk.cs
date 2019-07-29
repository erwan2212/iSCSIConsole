/* Copyright (C) 2016 Tal Aloni <tal.aloni.il@gmail.com>. All rights reserved.
 * 
 * You can redistribute this program and/or modify it under the terms of
 * the GNU Lesser Public License as published by the Free Software Foundation,
 * either version 3 of the License, or (at your option) any later version.
 */
using System;
using System.Collections.Generic;
using System.Text;
using Utilities;

namespace DiskAccessLibrary
{
    public class RAMDisk : Disk
    {
        public const int BytesPerRAMDiskSector = 512;

        private byte[] m_diskBytes;

        /// <summary>
        /// A single-dimensional byte array cannot contain more than 0X7FFFFFC7 bytes (2047.999 MiB).
        /// https://msdn.microsoft.com/en-us/library/System.Array(v=vs.110).aspx
        /// or in a x64 env + .net 4.5, see https://stackoverflow.com/questions/39025934/where-to-set-gcallowverylargeobjects
        /// </summary>
        public RAMDisk(int size)
        //public RAMDisk(long size)
        {
            m_diskBytes = new byte[size]; //max is 2047 * 1024 * 1024
        }

        public void Free()
        {
            m_diskBytes = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        public override byte[] ReadSectors(long sectorIndex, int sectorCount)
        {
            return ByteReader.ReadBytes(m_diskBytes, (int)sectorIndex * BytesPerRAMDiskSector, sectorCount * BytesPerRAMDiskSector);
        }

        public override void WriteSectors(long sectorIndex, byte[] data)
        {
            ByteWriter.WriteBytes(m_diskBytes, (int)sectorIndex * BytesPerRAMDiskSector, data);
        }

        public override int BytesPerSector
        {
            get
            {
                return BytesPerRAMDiskSector;
            }
        }

        public override long Size
        {
            get
            {
                return m_diskBytes.Length;
            }
        }
    }
}
