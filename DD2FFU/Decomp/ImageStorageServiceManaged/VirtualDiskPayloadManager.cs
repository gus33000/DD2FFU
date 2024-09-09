// Decompiled with JetBrains decompiler
// Type: Microsoft.WindowsPhone.Imaging.VirtualDiskPayloadManager
// Assembly: ImageStorageServiceManaged, Version=10.0.0.0, Culture=neutral, PublicKeyToken=b3f029d4c9c2ec30
// MVID: BF244519-1EED-4829-8682-56E05E4ACE17
// Assembly location: C:\Users\gus33000\source\repos\DD2FFU\DD2FFU\libraries\imagestorageservicemanaged.dll

using System;
using System.Collections.Generic;
using System.Reflection;
using Decomp.Microsoft.WindowsPhone.ImageUpdate.Tools.Common;

namespace Decomp.Microsoft.WindowsPhone.Imaging
{
    internal class VirtualDiskPayloadManager : IDisposable
    {
        private bool _alreadyDisposed;
        private List<Tuple<VirtualDiskPayloadGenerator, ImageStorage>> _generators;
        private readonly IULogger _logger;
        private readonly ushort _numOfStores;
        private readonly bool _recovery;
        private readonly ushort _storeHeaderVersion;

        public VirtualDiskPayloadManager(IULogger logger, ushort storeHeaderVersion, ushort numOfStores, bool recovery)
        {
            _logger = logger;
            _storeHeaderVersion = storeHeaderVersion;
            _numOfStores = numOfStores;
            _generators = [];
            _recovery = recovery;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void AddStore(ImageStorage storage)
        {
            if (_storeHeaderVersion < 2 && _generators.Count > 1)
            {
                throw new ImageStorageException(string.Format(
                                "{0}: Cannot add more than one store to a FFU using v1 store header.",
                                MethodBase.GetCurrentMethod().Name));
            }

            _generators.Add(new Tuple<VirtualDiskPayloadGenerator, ImageStorage>(
                            new VirtualDiskPayloadGenerator(_logger, ImageConstants.PAYLOAD_BLOCK_SIZE, storage,
                                _storeHeaderVersion, _numOfStores, (ushort)(_generators.Count + 1), _recovery), storage));
        }

        public void Write(IPayloadWrapper payloadWrapper)
        {
            long payloadSize = 0;
            foreach (Tuple<VirtualDiskPayloadGenerator, ImageStorage> generator in _generators)
            {
                VirtualDiskPayloadGenerator payloadGenerator = generator.Item1;
                ImageStorage storage = generator.Item2;
                payloadGenerator.GenerateStorePayload(storage);
                payloadSize += payloadGenerator.TotalSize;
            }

            payloadWrapper.InitializeWrapper(payloadSize);
            _generators.ForEach(t => t.Item1.WriteMetadata(payloadWrapper));
            _generators.ForEach(t => t.Item1.WriteStorePayload(payloadWrapper));
            payloadWrapper.FinalizeWrapper();
        }

        ~VirtualDiskPayloadManager()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (_alreadyDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                _generators.ForEach(t => t.Item1.Dispose());
                _generators.Clear();
                _generators = null;
            }

            _alreadyDisposed = true;
        }
    }
}