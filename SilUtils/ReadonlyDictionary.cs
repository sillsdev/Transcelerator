// ---------------------------------------------------------------------------------------------
#region // Copyright (c) 2013, SIL International. All Rights Reserved.
// <copyright from='2013' to='2013' company='SIL International'>
//		Copyright (c) 2013, SIL International. All Rights Reserved.   
//    
//		Distributable under the terms of either the Common Public License or the
//		GNU Lesser General Public License, as specified in the LICENSING.txt file.
// </copyright> 
#endregion
// 
// File: ReadonlyDictionary.cs
// ---------------------------------------------------------------------------------------------
using System.Collections;
using System.Collections.Generic;

namespace SIL.Utils
{
    public interface IReadonlyDictionary<TKey, TValue> : IEnumerable<KeyValuePair<TKey, TValue>>
    {
        TValue this[TKey key] { get; }

        bool TryGetValue(TKey key, out TValue value);

        IEnumerable<TValue> Values { get; }

        IEnumerable<TKey> Keys { get; }
    }

	/// ----------------------------------------------------------------------------------------
	/// <summary>
	/// This provides a string enumeration for subkeys for SIL software that is used from FieldWorks
	/// of HKCU\Software\SIL.
	/// </summary>
	/// ----------------------------------------------------------------------------------------
    public class ReadonlyDictionary<TKey, TValue> : IReadonlyDictionary<TKey, TValue>
	{
	    protected readonly IDictionary<TKey, TValue> m_dictionary;

        public ReadonlyDictionary()
        {
            m_dictionary = new Dictionary<TKey, TValue>();
        }

        public ReadonlyDictionary(IDictionary<TKey, TValue> dictionary)
        {
            m_dictionary = dictionary;
        }

        #region Implementation of IRulesDictionary
        public TValue this[TKey key]
	    {
            get { return m_dictionary[key]; }
	    }

	    public bool TryGetValue(TKey key, out TValue value)
	    {
            return m_dictionary.TryGetValue(key, out value);
        }

	    public IEnumerable<TValue> Values
	    {
            get { return m_dictionary.Values; }
	    }
	    public IEnumerable<TKey> Keys
	    {
            get { return m_dictionary.Keys; }
	    }

	    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
	    {
            return m_dictionary.GetEnumerator();
	    }

	    IEnumerator IEnumerable.GetEnumerator()
	    {
	        return GetEnumerator();
	    }
        #endregion
    }
}
