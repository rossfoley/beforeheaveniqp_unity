using UnityEngine;
using System.Collections;

public class AudioModel : MonoBehaviour {
	string name;
	string owner;
	string genre;
	string url;
	string room_id;
	int elapsed_time;
	int duration;

	public AudioModel(string name, string owner, 
	                  string genre, string url, string room_id, int elapsed_time, int duration){
		this.name = name;
		this.owner = owner;
		this.genre = genre;
		this.url = url;
		this.room_id = room_id;
		this.elapsed_time = elapsed_time;
		this.duration = duration;
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

	public string Room_id {
		get {
			return room_id;
		}
		set {
			room_id = value;
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

	public int Duration {
		get {
			return duration;
		}
		set {
			duration = value;
		}
	}

}
