using System.Collections.Generic;
using UnityEngine;

public class Speaker : ScriptableObject
{
	[SerializeField] private string speakerName;
	[SerializeField] private Sprite speakerSprite;
	[SerializeField] private int relation;

	public string GetName()
	{
		return speakerName;
	}

	public Sprite GetSprite()
	{
		return speakerSprite;
	}

	public int GetRelation()
	{
		return relation;
	}

	public void SetRelation(int relation)
	{
	  this.relation += relation;
	}
}
