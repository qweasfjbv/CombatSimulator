using UnityEngine;

namespace Defense.Manager
{
	public class Managers : MonoBehaviour
	{
		private static Managers s_instance;
		public static Managers Instance { get { return s_instance; } }

		/** Managers **/
		private ResourceManager _resource = new ResourceManager();

		/** Properties **/
		public static ResourceManager Resource { get { return Instance._resource; } }

		public void Init()
		{
			if (s_instance == null)
			{
				s_instance = this;
				DontDestroyOnLoad(this.gameObject);
			}
			else
			{
				Destroy(this.gameObject);
				return;
			}

			s_instance._resource.Init();
		}


		void Awake()
		{
			Init();
		}

	}
}