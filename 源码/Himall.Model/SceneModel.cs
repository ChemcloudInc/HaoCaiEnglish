using System;
using System.Runtime.CompilerServices;

namespace Himall.Model
{
	public class SceneModel
	{
		private QR_SCENE_Type _type;

		public object Object
		{
			get;
			set;
		}

		public QR_SCENE_Type SceneType
		{
			get
			{
				return _type;
			}
			set
			{
                _type = value;
			}
		}

		public SceneModel(QR_SCENE_Type type)
		{
            _type = type;
		}

		public SceneModel(QR_SCENE_Type type, object obj)
		{
            _type = type;
            Object = obj;
		}
	}
}