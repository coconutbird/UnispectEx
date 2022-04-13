﻿using System;

using UnispectEx.Pe.Constants;
using UnispectEx.Util;

namespace UnispectEx.Pe {
    internal class ImageNtHeaders {
        private ImageNtHeaders() { }
        
        internal uint Signature { get; init; }
        internal ImageFileHeader FileHeader { get; init; }
        internal ImageOptionalHeader OptionalHeader { get; init; }
        
        internal static ImageNtHeaders Create(Memory memory, ulong address) {
            var reader = new MemoryReader(memory, address);

            var signature = reader.U32();

            if (signature != ImageConstants.NtSignature) {
                throw new InvalidOperationException("dos header invalid!");
            }

            var fileHeader = ImageFileHeader.Create(memory, address + 4);
            var optionalHeader = ImageOptionalHeader.Create(
                memory,
                address + sizeof(uint) + ImageConstants.FileHeaderSize,
                fileHeader.Machine == ImageConstants.MachineI386);

            return new ImageNtHeaders {
                Signature = signature,
                FileHeader = fileHeader,
                OptionalHeader = optionalHeader
            };
        }
    }
}