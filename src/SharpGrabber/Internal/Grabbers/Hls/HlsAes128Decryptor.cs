using DotNetTools.SharpGrabber.Hls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotNetTools.SharpGrabber.Internal.Grabbers.Hls
{
    internal class HlsAes128Decryptor
    {
        private readonly HlsKey _key;
        private TaskCompletionSource<byte[]> _getKeyBytes;

        public HlsAes128Decryptor(HlsKey key)
        {
            _key = key;
        }

        public async Task<Stream> WrapStreamAsync(Stream stream)
        {
            var keyBytes = await GetKeyBytes().ConfigureAwait(false);

            return new DecryptionStream(stream, keyBytes, _key.Iv);
        }

        private Task<byte[]> GetKeyBytes()
        {
            if (_getKeyBytes != null)
                return _getKeyBytes.Task;

            var fetchBytes = false;
            lock (this)
            {
                if (_getKeyBytes == null)
                {
                    _getKeyBytes = new TaskCompletionSource<byte[]>();
                    fetchBytes = true;
                }
            }

            if (!fetchBytes)
                return _getKeyBytes.Task;
            return DownloadKeyBytes();
        }

        private async Task<byte[]> DownloadKeyBytes()
        {
            using var client = HttpHelper.CreateClient();
            var bytes = await client.GetByteArrayAsync(_key.Uri).ConfigureAwait(false);
            _getKeyBytes.SetResult(bytes);
            return bytes;
        }

        private class DecryptionStream : Stream
        {
            private readonly Stream _stream;
            private readonly Stream _underlayingStream;
            private readonly RijndaelManaged _rijndael;
            private readonly ICryptoTransform _decryptor;

            public DecryptionStream(Stream underlayingStream, byte[] keyBytes, byte[] iv)
            {
                _underlayingStream = underlayingStream;
                _rijndael = new RijndaelManaged
                {
                    Key = keyBytes,
                    IV = iv,
                    Padding = PaddingMode.Zeros,
                };
                _decryptor = _rijndael.CreateDecryptor();
                _stream = new CryptoStream(_underlayingStream, _decryptor, CryptoStreamMode.Read);
            }

            public override bool CanRead => _stream.CanRead;

            public override bool CanSeek => _stream.CanSeek;

            public override bool CanWrite => _stream.CanWrite;

            public override long Length => _stream.Length;

            public override long Position
            {
                get => _stream.Position;
                set => _stream.Position = value;
            }

            public override void Flush()
                => _stream.Flush();

            public override int Read(byte[] buffer, int offset, int count)
                => _stream.Read(buffer, offset, count);

            public override long Seek(long offset, SeekOrigin origin)
                => _stream.Seek(offset, origin);

            public override void SetLength(long value)
                => _stream.SetLength(value);

            public override void Write(byte[] buffer, int offset, int count)
                => _stream.Write(buffer, offset, count);

            public override Task<int> ReadAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => _stream.ReadAsync(buffer, offset, count, cancellationToken);

            public override Task WriteAsync(byte[] buffer, int offset, int count, CancellationToken cancellationToken)
                => _stream.WriteAsync(buffer, offset, count, cancellationToken);

            public override Task FlushAsync(CancellationToken cancellationToken)
                => _stream.FlushAsync(cancellationToken);

            protected override void Dispose(bool disposing)
            {
                base.Dispose(disposing);
                _stream.Dispose();
                _decryptor.Dispose();
                _rijndael.Dispose();
                _underlayingStream.Dispose();
            }
        }
    }
}
