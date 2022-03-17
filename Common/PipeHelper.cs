using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public static class PipeHelper
    {
        public static async Task<List<string>> ReadFromPipeAsync(PipeReader reader)
        {
            List<string> results = new List<string>();
            while (true)
            {
                //读取管道
                ReadResult readResult = await reader.ReadAsync();
                //读取的缓冲区
                var buffer = readResult.Buffer;

                //一个空点
                SequencePosition? position = null;
                do
                {
                    //缓冲区首个\n字符点位
                    position = buffer.PositionOf((byte)',');
                    if (position != null)
                    {
                        //切割\n之前的出来（包括\n）
                        ReadOnlySequence<byte> readerOnlySequ = buffer.Slice(0, position.Value);
                        AddStringToList(results, readerOnlySequ);
                        //切割后未处理部分
                        buffer = buffer.Slice(buffer.GetPosition(1, position.Value));
                    }
                } while (position != null);
                if (readResult.IsCompleted && buffer.Length > 0)
                {
                    AddStringToList(results, buffer);
                }

                //推进管道
                reader.AdvanceTo(buffer.Start, buffer.End);
                if (readResult.IsCompleted)
                {
                    break;
                }
            }
            return results;
        }
        public static void AddStringToList(List<string> results, in ReadOnlySequence<byte> readOnlySequence)
        {
            ReadOnlySpan<byte> span = readOnlySequence.IsSingleSegment ? readOnlySequence.FirstSpan : readOnlySequence.ToArray().AsSpan<byte>();
            results.Add(Encoding.UTF8.GetString(span));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="writer"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static async Task WriteToPipeAsync(PipeWriter writer, string msg)
        {
            byte[] buffer = Encoding.UTF8.GetBytes(msg);
            //先请求一块内存
            var memory = writer.GetMemory(buffer.Length);
            //把数据复制入该内存
            buffer.CopyTo(memory);
            //推进管道
            writer.Advance(buffer.Length);

            await writer.FlushAsync();
        }
    }
}
