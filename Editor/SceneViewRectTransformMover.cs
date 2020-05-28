using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Kogane.Internal
{
	/// <summary>
	/// UI オブジェクトが選択されている時に Scene ビューに 1 pixel 単位で移動させるボタンを表示するエディタ拡張
	/// </summary>
	[InitializeOnLoad]
	internal static class SceneViewRectTransformMover
	{
		//================================================================================
		// 定数
		//================================================================================
		private const float WIDTH    = 88f;
		private const float HEIGHT   = 88f;
		private const float OFFSET_X = 6;
		private const float OFFSET_Y = 26;

		//================================================================================
		// 関数(static)
		//================================================================================
		/// <summary>
		/// コンストラクタ
		/// </summary>
		static SceneViewRectTransformMover()
		{
			SceneView.duringSceneGui -= OnSceneGUI;
			SceneView.duringSceneGui += OnSceneGUI;
		}

		/// <summary>
		/// Scene ビューを描画する時に呼び出されます
		/// </summary>
		private static void OnSceneGUI( SceneView sceneView )
		{
			if ( !sceneView.camera.orthographic ) return;

			var mode      = SelectionMode.Editable | SelectionMode.ExcludePrefab;
			var selection = Selection.GetFiltered<RectTransform>( mode );

			if ( selection.Length <= 0 ) return;

			Handles.BeginGUI();

			var clientRect = new Rect
			(
				x: Screen.width - WIDTH - OFFSET_X,
				y: Screen.height - HEIGHT - OFFSET_Y,
				width: WIDTH,
				height: HEIGHT
			);

			GUI.Window
			(
				id: -1,
				clientRect: clientRect,
				func: id => OnWindow( selection )
			  , text: "uGUI"
			);

			Handles.EndGUI();
		}

		/// <summary>
		/// Scene ビューにウィンドウを表示します
		/// </summary>
		private static void OnWindow( IReadOnlyList<RectTransform> selection )
		{
			if ( GUILayout.Button( "↑" ) )
			{
				Move( Vector2.up, selection );
			}

			using ( new EditorGUILayout.HorizontalScope() )
			{
				if ( GUILayout.Button( "←" ) )
				{
					Move( Vector2.left, selection );
				}

				if ( GUILayout.Button( "→" ) )
				{
					Move( Vector2.right, selection );
				}
			}

			if ( GUILayout.Button( "↓" ) )
			{
				Move( Vector2.down, selection );
			}
		}

		/// <summary>
		/// 選択中の uGUI のオブジェクトを移動します
		/// </summary>
		private static void Move( Vector2 movement, IReadOnlyList<RectTransform> selection )
		{
			Event.current.Use();
			
			Undo.RecordObjects( selection.OfType<Object>().ToArray(), "Move UI" );

			foreach ( var rectTransform in selection )
			{
				var pos = rectTransform.anchoredPosition;
				pos                            += movement;
				pos.x                          =  Mathf.RoundToInt( pos.x );
				pos.y                          =  Mathf.RoundToInt( pos.y );
				rectTransform.anchoredPosition =  pos;
			}
		}
	}
}