using UnityEngine;
using System.Collections;

public static class Telemetry {

	private static SocketServer serverHandle;
	private static RemoteScreenCapture captureCam = null;

	public static SocketServer Server
	{
		get
		{
			if (serverHandle == null)
			{
				serverHandle = new SocketServer(60601);
				serverHandle.Start();
			}
			return serverHandle;
		}
	}

	public enum Level {
		Trace,
		Debug,
		Info,
		Warn,
		Error,
		Fatal
	}

	private static string[] logLevelTokens = {
		"trace",
		"debug",
		"info",
		"warn",
		"error",
		"fatal"
	};

	public static void log(Level level, string msg) {
		var json = "{\"type\": \"core.simpleLog.message\", \"payload\": {\"level\": \""
			+ logLevelTokens[(int)level]
			+ "\", \"message\": \""
			+ msg
			+ "\"}}";

		Server.Broadcast(json);
	}

	public static void trace(string msg) {
		log (Level.Trace, msg);
	}

	public static void debug(string msg) {
		log (Level.Debug, msg);
	}

	public static void info(string msg) {
		log (Level.Info, msg);
	}

	public static void warn(string msg) {
		log (Level.Warn, msg);
	}

	public static void error(string msg) {
		log (Level.Error, msg);
	}

	public static void fatal(string msg) {
		log (Level.Fatal, msg);
	}

	public static void scalar(string name, float value) {

		var json = "{\"type\": \"scalar\", \"payload\": {" +
				"\"name\": \"" + name + "\", " + 
				"\"value\": " + value + "}}";

		Server.Broadcast(json);
	}

	public static void data(string name, float value) {
		var json = "{\"type\": \"core.simpleTelemetry.sample\", \"payload\": {" +
			"\"name\": \"" + name + "\", " + 
			"\"time\": " + Time.time + ", " + 
			"\"value\": " + value + "}}";
		
		Server.Broadcast(json);
	}

	public static void registerCamera(RemoteScreenCapture cap) {
		captureCam = cap;
		Debug.Log("remote camera registered");
	}

	public static void mapTileRequest(Rect rect) {
		Debug.Log("mapTileRequest for: " + rect);
		captureCam.captureMapTile(rect);
	}

	public static void mapTile(byte[] bytes, Rect rect) {

		string base64 = System.Convert.ToBase64String(bytes);

		string json = "{\"type\": \"ext.simpleTelemetry.mapTileData\", \"payload\": {" +
			"\"mime\": \"image/png\", " +
			"\"encoding\": \"base64\", " +
			"\"data\": \"" + base64 + "\", " +
			"\"x\": "+ rect.xMin + ", " +
			"\"y\": " + rect.yMin + ", " +
			"\"width\": " + rect.width + ", " +
			"\"height\": " + rect.height + "}}";

		Server.Broadcast(json);
	}

	public static void position(Vector3 position) {
		string json = "{\"type\": \"ext.simpleTelemetry.location\", \"payload\": {" +
			"\"time\": " + Time.time + ", " + 
			"\"x\": " + position.x + ", " + 
			"\"y\": " + position.y + ", " + 
			"\"z\": " + position.z + "}}";

		Server.Broadcast(json);
	}
}
