// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.DataBlockStream
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.IO;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class DataBlockStream : Stream
    {
        private int _blockIndex = int.MaxValue;
        private DataBlockEntry _currentEntry;
        private long _position;

        public DataBlockStream(IBlockStreamSource streamSource, uint bytesPerBlock)
        {
            BytesPerBlock = bytesPerBlock;
            EntryLookupTable = [];
            BlockEntries = [];
            Source = streamSource;
        }

        public List<DataBlockEntry> BlockEntries
        {
            get;
        }

        private uint BytesPerBlock
        {
            get;
        }

        internal SortedDictionary<int, int> EntryLookupTable
        {
            get; set;
        }

        private IBlockStreamSource Source
        {
            get;
        }

        public override bool CanRead => true;

        public override bool CanWrite => true;

        public override bool CanSeek => true;

        public override bool CanTimeout => false;

        public override long Length => Source.Length;

        public override long Position
        {
            get => _position;
            set
            {
                if (value > Length)
                {
                    throw new ImageStorageException("The given position is beyond the end of the image payload.");
                }

                _position = value;
            }
        }

        private int BlockIndexFromStreamPosition => _position / BytesPerBlock > int.MaxValue
                    ? throw new ImageStorageException("The stream position is outside the addressable block range.")
                    : (int)(_position / BytesPerBlock);

        public override void Flush()
        {
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            if (offset > Length)
            {
                throw new ImageStorageException("The  offset is beyond the end of the image.");
            }

            switch (origin)
            {
                case SeekOrigin.Begin:
                    _position = offset;
                    return _position;
                case SeekOrigin.Current:
                    if (offset == 0L)
                    {
                        return _position;
                    }

                    if (offset < 0L)
                    {
                        throw new ImageStorageException("Negative offsets are not implemented.");
                    }

                    if (_position >= Length)
                    {
                        throw new ImageStorageException("The offset is beyond the end of the image.");
                    }

                    if (Length - _position < offset)
                    {
                        throw new ImageStorageException("The offset is beyond the end of the image.");
                    }

                    _position = offset;
                    return _position;
                case SeekOrigin.End:
                    if (offset > 0L)
                    {
                        throw new ImageStorageException("The offset is beyond the end of the image.");
                    }

                    if (Length + offset < 0L)
                    {
                        throw new ImageStorageException("The offset is invalid.");
                    }

                    _position = Length + offset;
                    return _position;
                default:
                    throw new ImageStorageException("The origin parameter is invalid.");
            }
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int num = 0;
            do
            {
                int length = (int)Math.Min(BytesPerBlock - (_position % BytesPerBlock), count - num);
                int fromStreamPosition = BlockIndexFromStreamPosition;
                if (_blockIndex != fromStreamPosition)
                {
                    if (!EntryLookupTable.ContainsKey(fromStreamPosition))
                    {
                        DataBlockEntry dataBlockEntry = new(BytesPerBlock);
                        dataBlockEntry.DataSource.Source = DataBlockSource.DataSource.Memory;
                        byte[] newMemoryData = dataBlockEntry.DataSource.GetNewMemoryData(BytesPerBlock);
                        Source.ReadBlock((uint)fromStreamPosition, newMemoryData, 0);
                        dataBlockEntry.BlockLocationsOnDisk.Add(new DiskLocation((uint)fromStreamPosition,
                            DiskLocation.DiskAccessMethod.DiskBegin));
                        BlockEntries.Add(dataBlockEntry);
                        EntryLookupTable.Add(fromStreamPosition, BlockEntries.Count - 1);
                        _currentEntry = dataBlockEntry;
                    }
                    else
                    {
                        _currentEntry = BlockEntries[EntryLookupTable[fromStreamPosition]];
                    }

                    _blockIndex = fromStreamPosition;
                }

                Array.Copy(_currentEntry.DataSource.GetMemoryData(), (int)(_position % BytesPerBlock), buffer, offset,
                    length);
                offset += length;
                num += length;
                _position += length;
            } while (num < count);

            return num;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (offset + Position > Length)
            {
                throw new EndOfStreamException("Cannot write past the end of the stream.");
            }

            int num = 0;
            do
            {
                int length = (int)Math.Min(BytesPerBlock - (_position % BytesPerBlock), count - num);
                int fromStreamPosition = BlockIndexFromStreamPosition;
                if (!EntryLookupTable.ContainsKey(fromStreamPosition))
                {
                    throw new ImageStorageException(
                                        "Attempting to write to an unallocated block data stream location.");
                }

                if (fromStreamPosition != _blockIndex)
                {
                    _currentEntry = BlockEntries[EntryLookupTable[fromStreamPosition]];
                    _blockIndex = fromStreamPosition;
                }

                int destinationIndex = (int)(_position % BytesPerBlock);
                Array.Copy(buffer, offset, _currentEntry.DataSource.GetMemoryData(), destinationIndex, length);
                offset += length;
                num += length;
                _position += length;
            } while (num < count);
        }
    }
}