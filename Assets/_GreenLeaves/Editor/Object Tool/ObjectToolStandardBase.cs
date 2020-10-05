using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class StandardToolSettings
{
	public KeyCode increaseBrushSizeKey;
	public KeyCode decreaseBrushSizeKey;

	public Color brushRadiusColor;
	public Color brushCenterColor;
}

public class ObjectToolStandardBase<StandardSettingsType> : ObjectToolAssetBase where StandardSettingsType : StandardToolSettings
{
	public StandardSettingsType settings;

	[Space]
	[Space]

	public float brushRadius;

	private bool canPressLeftBrushSize;
	private bool canPressRightBrushSize;

	public override void RunTool(RaycastHit hit, Event currentEvent, SceneView screenView, Transform placedObjectRoot)
	{
		DrawBrush(hit, screenView);
		ChangeBrushSize(currentEvent);
	}

	public void DrawBrush(RaycastHit hit, SceneView screenView)
	{
		Handles.color = settings.brushRadiusColor;
		Handles.DrawWireDisc(hit.point, hit.normal, brushRadius);
		//DebugExtension.DebugWireSphere(hit.point, settings.brushCenterColor);
		screenView.Repaint();
	}

	public void ChangeBrushSize(Event currentEvent)
	{
		if (currentEvent.type == EventType.KeyDown)
		{
			if (currentEvent.keyCode == settings.decreaseBrushSizeKey)
			{
				if (canPressLeftBrushSize)
				{
					canPressLeftBrushSize = false;
				}

				brushRadius -= 10;

				if (brushRadius < 0)
				{
					brushRadius = 0;
				}
			}
			else if (currentEvent.keyCode == settings.increaseBrushSizeKey)
			{
				if (canPressRightBrushSize)
				{
					canPressRightBrushSize = false;
				}

				brushRadius += 10;
			}
		}

		if (currentEvent.type == EventType.KeyUp)
		{
			if (currentEvent.keyCode == settings.decreaseBrushSizeKey)
			{
				canPressLeftBrushSize = true;
			}
			else if (currentEvent.keyCode == settings.increaseBrushSizeKey)
			{
				canPressRightBrushSize = true;
			}
		}
	}
}
