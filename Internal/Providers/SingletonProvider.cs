﻿using System;

namespace CryoDI.Providers
{
	internal class SingletonProvider<T> : IObjectProvider
	{
		private readonly Func<T> _factoryMethod;
		private bool _exist;
		private T _instance;

		public SingletonProvider(LifeTime lifeTime)
			: this(Activator.CreateInstance<T>, lifeTime)
		{
		}

		public SingletonProvider(Func<T> factoryMethod, LifeTime lifeTime)
		{
			LifeTime = lifeTime;
			_factoryMethod = factoryMethod;
		}

		public LifeTime LifeTime { get; }

		public object GetObject(object owner, CryoContainer container, params object[] parameters)
		{
			if (!_exist)
			{
				_instance = _factoryMethod();
				_exist = true;

				container.BuildUp(_instance, parameters);
				LifeTimeManager.TryToAdd(this, LifeTime);
			}

			return _instance;
		}

		public object WeakGetObject(CryoContainer container, params object[] parameters)
		{
			if (_exist)
				return _instance;

			return null;
		}

		public void Dispose()
		{
			if (!_exist)
				return;

			if (LifeTime != LifeTime.External)
			{
				var disposable = _instance as IDisposable;
				if (disposable != null)
					disposable.Dispose();
			}

			_instance = default(T);
			_exist = false;
		}
	}
}