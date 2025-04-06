using UnityEngine;

namespace IUtil.Utils
{
	public static class IUtilDebug
	{
		public static void NoFieldError(string attr, string fieldName)
		{
			Debug.LogError($"IUtil Error : [{attr}] can't find field: {fieldName}");
		}

		public static void TypeError(string attr, string wrongType)
		{
			Debug.LogError($"IUtil Error : [{attr}] can't matching type: {wrongType}");
		}

		public static void ParameterCountError(string attr, string funcName)
		{
			Debug.LogError($"IUtil Error : [{attr}] - {funcName} \nThe number of parameters does not match.");
		}
	}
}
