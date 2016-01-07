using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MapsDropdown : MonoBehaviour {

	private Dropdown dropdown;

	public List<string> maps;

	void Start () {
		dropdown = GetComponent<Dropdown>();

		dropdown.AddOptions(maps);
	}

}
