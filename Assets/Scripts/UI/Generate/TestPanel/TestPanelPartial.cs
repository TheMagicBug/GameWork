using UnityEngine;
using UnityEngine.UI;

//自动生成于：2023/5/25 16:56:14
namespace SQDFC
{

	public partial class TestPanel
	{

		private Image m_Img_TEST;

		private void GetBindComponents(GameObject go)
		{
			ComponentAutoBindTool autoBindTool = go.GetComponent<ComponentAutoBindTool>();

			m_Img_TEST = autoBindTool.GetBindComponent<Image>(0);
		}
	}
}
