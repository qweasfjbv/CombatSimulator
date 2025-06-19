using UnityEngine;

namespace Autobattler.Manager
{
	public class Managers : MonoBehaviour
	{
		private static Managers s_instance;
		public static Managers Instance { get { return s_instance; } }

		/** Managers **/
		private ResourceManager _resource = new ResourceManager();
		private InputManager _input = new InputManager();
		private DamageLogManager _damageLog = new DamageLogManager();

		/** Properties **/
		public static ResourceManager Resource { get { return Instance._resource; } }
		public static InputManager Input { get { return Instance._input; } }
		public static DamageLogManager DamageLog { get { return Instance._damageLog; } }

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
			s_instance._input.Init();
			s_instance._damageLog.Init();
		}


		private void Awake()
		{
			Init();
		}

		private void Update()
		{
			s_instance._input.OnUpdate();
		}
	}
}