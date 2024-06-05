using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PzenaAssessment.Models
{
    public class PriceDataReader : IDataReader
    {
        private readonly List<Price> _priceList;
        private int _currentIndex = -1;

        public PriceDataReader(List<Price> priceList)
        {
            _priceList = priceList;
        }

        public object this[int i] => GetValue(i);

        public object this[string name] => throw new NotImplementedException();

        public int Depth => throw new NotImplementedException();

        public bool IsClosed => false;

        public int RecordsAffected => throw new NotImplementedException();

        public int FieldCount => 10; // Assuming 10 columns in the Price object

        public void Close()
        {
            // No action needed
        }

        public void Dispose()
        {
            // No action needed
        }

        public bool GetBoolean(int i)
        {
            throw new NotImplementedException();
        }

        public byte GetByte(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public char GetChar(int i)
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public string GetDataTypeName(int i)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDateTime(int i)
        {
            if (_currentIndex < 0 || _currentIndex >= _priceList.Count)
                throw new InvalidOperationException("Invalid operation");

            var price = _priceList[_currentIndex];

            return i switch
            {
                1 => price.Date ?? DateTime.MinValue,
                9 => price.LastUpdated ?? DateTime.MinValue,
                _ => throw new IndexOutOfRangeException($"Index {i} is out of range"),
            };
        }
        public double GetDouble(int i)
        {
            if (_currentIndex < 0 || _currentIndex >= _priceList.Count)
                throw new InvalidOperationException("Invalid operation");

            var price = _priceList[_currentIndex];

            return i switch
            {
                // Adjust the indices to match the position of the fields in your Price object
                2 => price.OpenPrice.HasValue ? Convert.ToDouble(price.OpenPrice.Value) : 0.0,
                3 => price.HighPrice.HasValue ? Convert.ToDouble(price.HighPrice.Value) : 0.0,
                4 => price.LowPrice.HasValue ? Convert.ToDouble(price.LowPrice.Value) : 0.0,
                5 => price.ClosePrice.HasValue ? Convert.ToDouble(price.ClosePrice.Value) : 0.0,
                6 => price.Volume.HasValue ? Convert.ToDouble(price.Volume.Value) : 0.0,
                7 => price.CloseAdj.HasValue ? Convert.ToDouble(price.CloseAdj.Value) : 0.0,
                8 => price.CloseUnadj.HasValue ? Convert.ToDouble(price.CloseUnadj.Value) : 0.0,
                _ => throw new IndexOutOfRangeException($"Index {i} is out of range"),
            };
        }

        public decimal GetDecimal(int i)
        {
            throw new NotImplementedException();
        }

        //public double GetDouble(int i)
        //{
        //    throw new NotImplementedException();
        //}

        public Type GetFieldType(int i)
        {
            return i switch
            {
                1 or 9 => typeof(DateTime),
                2 or 3 or 4 or 5 or 6 or 7 or 8 => typeof(double),
                _ => throw new IndexOutOfRangeException($"Index {i} is out of range"),
            };
        }

        public float GetFloat(int i)
        {
            throw new NotImplementedException();
        }

        public Guid GetGuid(int i)
        {
            throw new NotImplementedException();
        }

        public short GetInt16(int i)
        {
            throw new NotImplementedException();
        }

        public int GetInt32(int i)
        {
            throw new NotImplementedException();
        }

        public long GetInt64(int i)
        {
            throw new NotImplementedException();
        }

        public string GetName(int i)
        {
            return i switch
            {
                0 => "Ticker",
                1 => "Date",
                2 => "OpenPrice",
                3 => "HighPrice",
                4 => "LowPrice",
                5 => "ClosePrice",
                6 => "Volume",
                7 => "CloseAdj",
                8 => "CloseUnadj",
                9 => "LastUpdated",
                _ => throw new IndexOutOfRangeException($"Index {i} is out of range"),
            };
        }

        public int GetOrdinal(string name)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public string GetString(int i)
        {
            throw new NotImplementedException();
        }

        public object GetValue(int i)
        {
            if (_currentIndex < 0 || _currentIndex >= _priceList.Count)
                throw new InvalidOperationException("Invalid operation");

            var price = _priceList[_currentIndex];

            return i switch
            {
                0 => price.Ticker,
                1 => price.Date,
                2 => price.OpenPrice,
                3 => price.HighPrice,
                4 => price.LowPrice,
                5 => price.ClosePrice,
                6 => price.Volume,
                7 => price.CloseAdj,
                8 => price.CloseUnadj,
                9 => price.LastUpdated,
                _ => throw new IndexOutOfRangeException($"Index {i} is out of range"),
            };
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public bool IsDBNull(int i)
        {
            return GetValue(i) == null || GetValue(i) == DBNull.Value;
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public bool Read()
        {
            _currentIndex++;
            return _currentIndex < _priceList.Count;
        }
    }
}
