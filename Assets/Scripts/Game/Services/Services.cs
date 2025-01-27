﻿namespace Tartaros.ServicesLocator
{
	using System;
	using System.Collections.Generic;
	
	using UnityEngine;

	public class Services : Singleton<Services>
	{
		#region Fields
		private const string DBG_ERROR_DB_NOT_REGISTER = "Cannot get the database of type {0}: it has not be registed. Please, register it with the method {1}.";
		private const string DBG_ERROR_ALREADY_INITIALIZED = "Initialization of DatabaseLocator aborted: it is already initialized.";		

		private Dictionary<Type, object> _databases = new Dictionary<Type, object>();
		private bool _isInitialized = false;
		#endregion Fields

		#region Methods
		public T Get<T>()
		{
			if (_databases.ContainsKey(typeof(T)) == false)
			{
				throw new MissingServiceException<T>();
			}

			return (T)_databases[typeof(T)];
		}

        public bool TryGet<T>(out T service)
		{
			if (HasService<T>())
			{
				service = Get<T>();
				return true;
			}
			else
			{
				service = default;
				return false;
			}
		}

		public bool HasService<T>()
		{
			return _databases.ContainsKey(typeof(T));
		}

		public void RegisterService<T>(T database)
		{
			if (HasService<T>() == true)
			{
				_databases[typeof(T)] = database;
			}
			else
			{
				_databases.Add(typeof(T), database);
			}
		}

		public void UnregisterService<T>()
		{
			if (HasService<T>() == true)
			{
				_databases.Remove(typeof(T));
			}
			else
			{
				Debug.LogErrorFormat("Cannot unregister service {0}: no corresponding service was registered.", typeof(T));
			}
		}

		public void Initialize()
		{
			if (_isInitialized == true)
			{
				Debug.LogError(DBG_ERROR_ALREADY_INITIALIZED);
				return;
			}

			_isInitialized = true;

			ClearServices();
		}

		public void ClearServices()
		{
			_databases.Clear();
		}
		#endregion Methods
	}
}
