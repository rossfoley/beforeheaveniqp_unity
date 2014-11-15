using UnityEngine;
using System.Collections;

public class AudioModel : MonoBehaviour {
	string name;
	string owner;
	string genre;
	string url;
	int elapsed_time;

	public AudioModel(string name, string owner, 
	                  string genre, string url, int elapsed_time){
		this.name = name;
		this.owner = owner;
		this.genre = genre;
		this.url = url;
		this.elapsed_time = elapsed_time;
	}

	public override string ToString ()
	{
		return string.Format ("[AudioModel: Name={0}, Owner={1}, Genre={2}, Url={3}, Elapsed_time={4}]", Name, Owner, Genre, Url, Elapsed_time);
	}

	public string Name {
		get {
			return name;
		}
		set {
			name = value;
		}
	}

	public string Owner {
		get {
			return owner;
		}
		set {
			owner = value;
		}
	}

	public string Genre {
		get {
			return genre;
		}
		set {
			genre = value;
		}
	}

	public string Url {
		get {
			return url;
		}
		set {
			url = value;
		}
	}

	public int Elapsed_time {
		get {
			return elapsed_time;
		}
		set {
			elapsed_time = value;
		}
	}
}
