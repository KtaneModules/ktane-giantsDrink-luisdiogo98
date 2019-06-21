using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using System.Text.RegularExpressions;

public class giantsDrinkScript : MonoBehaviour 
{
	public KMBombInfo bomb;
	public KMAudio Audio;

	static System.Random rnd = new System.Random();

	public KMSelectable btnLeft, btnRight;
	public Material[] materials;
	public Material[] liquids;
	public Material[] gemMat;
	public GameObject[] leftGoblets, rightGoblets;
	public GameObject[] leftLiquids, rightLiquids;
	public GameObject[] leftGems, rightGems;


	int[] shape = new int[2];
	int[] mat = new int[2];
	int[] gems = new int[2];
	int[] liquid = new int[2];

	int evenStrikes;
	int oddStrikes;

	List<int>[] order = new List<int>[2];

	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved = false;

	void Awake()
	{
		moduleId = moduleIdCounter++;

		btnLeft.OnInteract += delegate () { HandleBtn(0, btnLeft); return false; };
		btnRight.OnInteract += delegate () { HandleBtn(1, btnRight); return false; };
	}

	void Start () 
	{
		order[0] = new List<int>();
		order[1] = new List<int>();

		CreateGoblets();
		ShowGoblets();
		Solve();

		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The key sequence for even strikes is {1}.", moduleId, String.Join(",", order[0].ToArray().Select(p => p.ToString()).ToArray()));
		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The key sequence for odd strikes is {1}.", moduleId, String.Join(",", order[1].ToArray().Select(p => p.ToString()).ToArray()));

		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The solution for even strikes is {1}.", moduleId, evenStrikes == 0 ? "left goblet" : "right goblet");
		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The solution for odd strikes is {1}.", moduleId, oddStrikes == 0 ? "left goblet" : "right goblet");

	}
	
	void Update () 
	{
		
	}

	void HandleBtn(int btn, KMSelectable obj)
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		obj.AddInteractionPunch(.5f);

		if(moduleSolved)
			return;

		int strikes = bomb.GetStrikes() % 2;

		if((strikes == 0 && evenStrikes == btn) | (strikes == 1 && oddStrikes == btn))
			moduleSolved = true;

		if(moduleSolved)
		{
			if(btn == 0)
				leftLiquids[shape[0]].SetActive(false);
			else
				rightLiquids[shape[1]].SetActive(false);

			GetComponent<KMBombModule>().HandlePass();
			UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] Solved! Pressed the {1} goblet (strikes were {2}).", moduleId, btn == 0 ? "left" : "right", strikes == 0 ? "even" : "odd");
		}
		else
		{
			GetComponent<KMBombModule>().HandleStrike();
			UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] Strike! Pressed the {1} goblet (strikes were {2}).", moduleId, btn == 0 ? "left" : "right", strikes == 0 ? "even" : "odd");
			
			RefillGoblets();
			Solve();

			UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The key sequence for even strikes is {1}.", moduleId, String.Join(",", order[0].ToArray().Select(p => p.ToString()).ToArray()));
			UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The key sequence for odd strikes is {1}.", moduleId, String.Join(",", order[1].ToArray().Select(p => p.ToString()).ToArray()));

			UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The solution for even strikes is {1}.", moduleId, evenStrikes == 0 ? "left goblet" : "right goblet");
			UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The solution for odd strikes is {1}.", moduleId, oddStrikes == 0 ? "left goblet" : "right goblet");
		}
	}

	void CreateGoblets()
	{
		shape[0] = rnd.Next() % 8;
		shape[1] = rnd.Next() % 8;

		mat[0] = rnd.Next() % 4;
		mat[1] = rnd.Next() % 4;

		gems[0] = rnd.Next() % 8;
		gems[1] = rnd.Next() % 8;

		liquid[0] = rnd.Next() % 6;
		liquid[1] = rnd.Next() % 6;

		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The left goblet is made of {1}, its gems are {2} and its liquid is {3} (Size: {4}; Gem Placement: {5}, Gem Count: {6}).", moduleId, GetMaterial(mat[0]), GetGems(gems[0]), GetLiquid(liquid[0]), GetSize(shape[0]), GetGemPosition(shape[0]), GetGemCount(shape[0]));
		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The right goblet is made of {1}, its gems are {2} and its liquid is {3} (Size: {4}; Gem Placement: {5}, Gem Count: {6}).", moduleId, GetMaterial(mat[1]), GetGems(gems[1]), GetLiquid(liquid[1]), GetSize(shape[1]), GetGemPosition(shape[1]), GetGemCount(shape[1]));
		
	}

	void ShowGoblets()
	{
		leftGoblets[shape[0]].SetActive(true);
		rightGoblets[shape[1]].SetActive(true);

		foreach(Renderer part in leftGoblets[shape[0]].GetComponentsInChildren<Renderer>())
		{
			part.material = materials[mat[0]];
		}

		foreach(Renderer part in rightGoblets[shape[1]].GetComponentsInChildren<Renderer>())
		{
			part.material = materials[mat[1]];
		}

		leftLiquids[shape[0]].GetComponentsInChildren<Renderer>()[0].material = liquids[liquid[0]];
		rightLiquids[shape[1]].GetComponentsInChildren<Renderer>()[0].material = liquids[liquid[1]];

		foreach(Renderer part in leftGems[shape[0]].GetComponentsInChildren<Renderer>())
		{
			part.material = gemMat[gems[0]];
		}

		foreach(Renderer part in rightGems[shape[1]].GetComponentsInChildren<Renderer>())
		{
			part.material = gemMat[gems[1]];
		}
	}

	void RefillGoblets()
	{
		liquid[0] = rnd.Next() % 6;
		liquid[1] = rnd.Next() % 6;

		leftLiquids[shape[0]].GetComponentInChildren<Renderer>().material = liquids[liquid[0]];
		rightLiquids[shape[1]].GetComponentInChildren<Renderer>().material = liquids[liquid[1]];

		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The left goblet's liquid is now {1}.", moduleId, GetLiquid(liquid[0]));
		UnityEngine.Debug.LogFormat("[The Giant's Drink #{0}] The right goblet's liquid is now {1}.", moduleId, GetLiquid(liquid[1]));
	}

	void Solve()
	{
		evenStrikes = Key1(0);
		oddStrikes = Key1(1);
	}

	int Key1(int goblet)
	{
		order[goblet].Add(1);

		if(mat[goblet] == 0 || mat[goblet] == 1)
			return Key2(goblet);
		else
			return Key17(goblet);
	}

	int Key2(int goblet)
	{
		order[goblet].Add(2);

		if(shape[goblet] == 0 || shape[goblet] == 2 || shape[goblet] == 6 || shape[goblet] == 7)
			return Key3(goblet);
		else
			return Key10(goblet);
	}

	int Key3(int goblet)
	{
		order[goblet].Add(3);

		if(gems[goblet] == 0 || gems[goblet] == 1 || gems[goblet] == 2)
			return Key4(goblet);
		else
			return Key7(goblet);
	}

	int Key4(int goblet)
	{
		order[goblet].Add(4);

		if(mat[0] == mat[1])
			return Key5(goblet);
		else
			return Key6(goblet);
	}

	int Key5(int goblet)
	{
		order[goblet].Add(5);

		if(liquid[goblet] == 0 || liquid[goblet] == 1)
			return goblet;
		else
			return (goblet - 1) * (-1);
	}

	int Key6(int goblet)
	{
		order[goblet].Add(6);

		if(liquid[goblet] == 2 || liquid[goblet] == 3)
			return 1;
		else
			return 0;
	}

	int Key7(int goblet)
	{
		order[goblet].Add(7);

		if(liquid[goblet] == 4 || liquid[goblet] == 5)
			return Key8(goblet);
		else
			return Key9(goblet);
	}

	int Key8(int goblet)
	{
		order[goblet].Add(8);

		if(gems[goblet] != 4 && gems[goblet] - 1 == liquid[goblet])
			return 0;
		else
			return 1;
	}

	int Key9(int goblet)
	{
		order[goblet].Add(9);

		if(shape[0] == shape[1])
			return (goblet - 1) * (-1);
		else
			return goblet;
	}

	int Key10(int goblet)
	{
		order[goblet].Add(10);

		if(shape[goblet] > shape[(goblet - 1) * (-1)])
			return Key11(goblet);
		else
			return Key14(goblet);
	}

	int Key11(int goblet)
	{
		order[goblet].Add(11);

		if(gems[goblet] == 3 || gems[goblet] == 4 || gems[goblet] == 5)
			return Key12(goblet);
		else
			return Key13(goblet);
	}

	int Key12(int goblet)
	{
		order[goblet].Add(12);

		if(liquid[goblet] == 0 || liquid[goblet] == 3)
			return 1;
		else
			return 0;
	}

	int Key13(int goblet)
	{
		order[goblet].Add(13);

		if(liquid[goblet] == 2 || liquid[goblet] == 4)
			return (goblet - 1) * (-1);
		else
			return goblet;
	}

	int Key14(int goblet)
	{
		order[goblet].Add(14);

		if(shape[goblet] == 0 || shape[goblet] == 1 || shape[goblet] == 4 || shape[goblet] == 7)
			return Key15(goblet);
		else
			return Key16(goblet);
	}

	int Key15(int goblet)
	{
		order[goblet].Add(15);

		if(gems[goblet] == 0 || gems[goblet] == 6 || gems[goblet] == 7)
			return goblet;
		else
			return (goblet - 1) * (-1);
	}

	int Key16(int goblet)
	{
		order[goblet].Add(16);

		if(gems[0] == gems[1])
			return 0;
		else
			return 1;
	}

	int Key17(int goblet)
	{
		order[goblet].Add(17);

		if(shape[goblet] < shape[(goblet - 1) * (-1)])
			return Key18(goblet);
		else
			return Key25(goblet);
	}

	int Key18(int goblet)
	{
		order[goblet].Add(18);

		if(liquid[goblet] == 1 || liquid[goblet] == 5)
			return Key19(goblet);
		else
			return Key22(goblet);
	}

	int Key19(int goblet)
	{
		order[goblet].Add(19);

		if(shape[goblet] == 1 || shape[goblet] == 3 || shape[goblet] == 4 || shape[goblet] == 5)
			return Key20(goblet);
		else
			return Key21(goblet);
	}

	int Key20(int goblet)
	{
		order[goblet].Add(20);

		if(gems[goblet] == 1 || gems[goblet] == 3 || gems[goblet] == 7)
			return 1;
		else
			return 0;
	}

	int Key21(int goblet)
	{
		order[goblet].Add(21);

		if(gems[goblet] == 2 || gems[goblet] == 4 || gems[goblet] == 6)
			return goblet;
		else
			return (goblet - 1) * (-1);
	}

	int Key22(int goblet)
	{
		order[goblet].Add(22);

		if(gems[goblet] == 1 || gems[goblet] == 5 || gems[goblet] == 7)
			return Key23(goblet);
		else
			return Key24(goblet);
	}

	int Key23(int goblet)
	{
		order[goblet].Add(23);

		if(mat[goblet] == 2)
			return (goblet - 1) * (-1);
		else
			return goblet;
	}

	int Key24(int goblet)
	{
		order[goblet].Add(24);

		if(mat[goblet] == 3)
			return 1;
		else
			return 0;
	}

	int Key25(int goblet)
	{
		order[goblet].Add(25);

		if(shape[goblet] == 2 || shape[goblet] == 3 || shape[goblet] == 5 || shape[goblet] == 6)
			return Key26(goblet);
		else
			return Key29(goblet);
	}

	int Key26(int goblet)
	{
		order[goblet].Add(26);

		if(gems[goblet] == 2 || gems[goblet] == 3 || gems[goblet] == 5)
			return Key27(goblet);
		else
			return Key28(goblet);
	}

	int Key27(int goblet)
	{
		order[goblet].Add(27);

		if(liquid[goblet] == 0 || liquid[goblet] == 4)
			return (goblet - 1) * (-1);
		else
			return goblet;
	}

	int Key28(int goblet)
	{
		order[goblet].Add(28);

		if(liquid[goblet] == 2 || liquid[goblet] == 5)
			return 0;
		else
			return 1;
	}

	int Key29(int goblet)
	{
		order[goblet].Add(29);

		if(liquid[goblet] == 1 || liquid[goblet] == 3)
			return Key30(goblet);
		else
			return Key31(goblet);
	}

	int Key30(int goblet)
	{
		order[goblet].Add(30);

		if(gems[goblet] == 0 || gems[goblet] == 4 || gems[goblet] == 7)
			return 0;
		else
			return 1;
	}

	int Key31(int goblet)
	{
		order[goblet].Add(31);

		if(gems[goblet] == 1 || gems[goblet] == 5 || gems[goblet] == 6)
			return goblet;
		else
			return (goblet - 1) * (-1);
	}

	String GetMaterial(int mat)
	{
		switch(mat)
		{
			case 0:
				return "gold";
			case 1:
				return "silver";
			case 2:
				return "bronze";
			case 3:
				return "iron";
		}

		return "";
	}

	String GetGems(int gems)
	{
		switch(gems)
		{
			case 0:
				return "diamonds";
			case 1:
				return "rubies";
			case 2:
				return "sapphires";
			case 3:
				return "emeralds";
			case 4:
				return "ambers";
			case 5:
				return "amethysts";
			case 6:
				return "aquamarines";
			case 7:
				return "onyx";
		}

		return "";
	}

	String GetLiquid(int liquid)
	{
		switch(liquid)
		{
			case 0:
				return "red";
			case 1:
				return "blue";
			case 2:
				return "green";
			case 3:
				return "orange";
			case 4:
				return "purple";
			case 5:
				return "cyan";
		}

		return "";
	}

	int GetSize(int shape)
	{
		return 8 - shape;
	}

	String GetGemPosition(int shape)
	{
		if(shape == 0 || shape == 1 || shape == 4 || shape == 7)
			return "top";
		else
			return "bottom";
	}

	int GetGemCount(int shape)
	{
		switch(shape)
		{
			case 0:
				return 10;
			case 1:
				return 5;
			case 2:
				return 12;
			case 3:
				return 5;
			case 4:
				return 4;
			case 5:
				return 4;
			case 6:
				return 9;
			case 7:
				return 10;
		}

		return -1;
	}

    //twitch plays
    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} left [Drinks from the left goblet] | !{0} right [Drinks from the right goblet] | !{0} tilt u [General TP command, use to see contents of goblets]";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*left\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            yield return new[] { btnLeft };
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*right\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            yield return new[] { btnRight };
            yield break;
        }
    }
}
