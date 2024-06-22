using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BuildLand : CreatureLand
{
	public GameObject CurrentLand;

	// Override the InitializeNeighbors method
	protected override void InitializeNeighbors()
	{
		base.InitializeNeighbors();

		// Determine the current land and neighbors for the build lanes
		string buildName = this.gameObject.name;

		if (buildName.StartsWith("p1Build") || buildName.StartsWith("p2Build"))
		{
			int buildNumber = int.Parse(buildName.Substring(7)); // Get the number part of the build

			// Set the CurrentLand field
			if (buildName.StartsWith("p1Build"))
			{
				CurrentLand = GameObject.Find("p1Land" + buildNumber);
			}
			else if (buildName.StartsWith("p2Build"))
			{
				CurrentLand = GameObject.Find("p2Land" + buildNumber);
			}

			// Set the Across field for the build lanes
			if (buildName.StartsWith("p1Build"))
			{
				_Across = GameObject.Find("p2Build" + buildNumber);
			}
			else if (buildName.StartsWith("p2Build"))
			{
				_Across = GameObject.Find("p1Build" + buildNumber);
			}

			// Set the AdjacentLeft and AdjacentRight fields for the build lanes
			if (buildNumber > 1)
			{
				AdjacentLeft = GameObject.Find(buildName.Substring(0, 7) + (buildNumber - 1));
			}
			if (buildNumber < 4) // Assuming there are 4 builds per row
			{
				AdjacentRight = GameObject.Find(buildName.Substring(0, 7) + (buildNumber + 1));
			}

			// Set the DiagLeft and DiagRight fields for the build lanes
			if (buildName.StartsWith("p1Build"))
			{
				if (buildNumber > 1)
				{
					DiagLeft = GameObject.Find("p2Build" + (buildNumber - 1));
				}
				if (buildNumber < 4)
				{
					DiagRight = GameObject.Find("p2Build" + (buildNumber + 1));
				}
			}
			else if (buildName.StartsWith("p2Build"))
			{
				if (buildNumber > 1)
				{
					DiagLeft = GameObject.Find("p1Build" + (buildNumber - 1));
				}
				if (buildNumber < 4)
				{
					DiagRight = GameObject.Find("p1Build" + (buildNumber + 1));
				}
			}
		}
	}
}
