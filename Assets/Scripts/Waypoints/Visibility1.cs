//Different Speeds: optimal vs one strategy.3 speeds
//Area proportion: % of points vs strategy vs levels vs player path
//multiple levels: MGS level (total 3)
//Distribution grapgh: time saved vs num of points
//Related work: visibility vs shadow, stealth, hide and seek as game,
using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
//using System;
public partial class Visibility1 : MonoBehaviour {
	List<Geometry> obsGeos = new List<Geometry> (); 
	//Contains Map
	Geometry mapBG = new Geometry ();
	int m_nCurrDiscretePtIndxX=0;
	int m_nCurrDiscretePtIndxZ=0;
	float[,] timingArray = null;
	int[,] pointsArray = null;
	float maxTimeEvaded = -1.0f;
	float currRunTimeEnemny = 0.0f;
	float currRunPointsEnemy = 0.0f;
	string fileLastCaseExecutedFor = "C:\\Users\\Dhaliwal\\Desktop\\lastCaseExecutedFor1.txt";
	string fileTimings = "C:\\Users\\Dhaliwal\\Desktop\\timingScene1.txt";
	string filePoints = "C:\\Users\\Dhaliwal\\Desktop\\pointsScene1.txt";
	string file_proveShadowAssisted = "C:\\Users\\Dhaliwal\\Desktop\\proveShadowAssisted1.txt";
	string file_AgentBasedEachLevelFolder = "C:\\Users\\Dhaliwal\\Desktop\\AgentBasedEachLevel";
	bool bShowLogs=false;
	List<Geometry> globalPolygon;
	List<Vector3> pathPoints;
	float mapDiagonalLength = 0;
	GameObject floor ;
	public Camera camObj;
	// Use this for initialization
	GameObject spTemp ;
	float standardMaxMovement = -1.0f;
	List<GameObject> shadowMeshes;
	private float startTimePlayer;
	private float startTimeEnemy;
	private float journeyLength;
	public float speedPlayer = 1.0F;
	public float speedEnemy = 1.0F;
	GameObject playerObj;
	public GameObject playerPrefab;
	public GameObject enemyPrefab;
	int m_nEnemyStatic = 1;
	int m_nEnemyNearMiss = 1;
	int m_nEnemyGreedy = 1;
	//public int nearMissAlgo = 1;
	public bool m_SetUpCase = false;
	public bool m_ContinueCase = false;
	public bool m_EnemyStatic = false;
	public bool m_Greedy = false;
	public bool m_NearMiss = false;
	public bool m_ShadowEdgeAssisted = false;
	//public int setUpCase = -1;


	public bool bDisplayAreas = false;
	List<Vector3> enemyPath = null;
	int nextPlayerPath = 1;
	List<EnemyMovement> m_enemyNearMissList = new List<EnemyMovement>();
	List<EnemyMovement> m_enemyGreedyList = new List<EnemyMovement>();
	List<EnemyMovement> m_enemyShadowAssistedList = new List<EnemyMovement>();
	class EnemyMovement
	{
		public GameObject enemyObj;
		public bool bCaught;
		public Vector2 startPosIndx;
		public List<Vector3> vNextPos;
		public EnemyMovement()
		{
			vNextPos = new List<Vector3>();
			bCaught = false;
		}
		List<float[,]> timingArray = new List<float[,]> ();
		List<float[,]> pointsArray = new List<float[,]> ();
	}

	List<Geometry> globalTempShadowPoly = new List<Geometry>();
	Geometry globalTempStarPoly;
	List<List<Vector3>> globalTempintersectionPointsPerV = new List<List<Vector3>>();
	List<Line> globalTempAllShadowLines = new List<Line>();
	public Material mat;
	public GameObject pathSphere;
	public GameObject hiddenSphere;
	public GameObject selectedBoxPrefab;
	GameObject selectedBox;
	List<GameObject> hiddenSphereList;
	//GameObject shadowObject = new GameObject();
	
	
	Hashtable hTable;
	Hashtable hVisiblePolyTable;
	Hashtable hVisibleTrianglesTable;
	Hashtable hVisibleNewPolygons;
	Vector3 start_box,end_box;
	Rect boundbox;
	bool b_ShowBoundbox=false;

	/*public */int m_nEnemyCentroid = 0;
	List<EnemyMovement> m_enemyCentroidList = new List<EnemyMovement>();
	List<Vector3> m_enemyNextPosCentroidList = new List<Vector3>();
	public bool m_ExecuteTrueCase = false;

	public bool m_CalculateTrueCase = false;
	public bool m_DisplayOptimizedPaths  = false;
	public bool m_ShowTrueCase = false;
	private string currSceneName;
	//Distance b/w consecutive path points
	private float m_stepDistance = -1.0f;
	Hashtable hCentroidShadows = new Hashtable();
	float m_minX = 0f;
	float m_minZ = 0f;
	float m_maxX = 0f;
	float m_maxZ = 0f;
	public float m_step = 0.1f;
	float radius_enemy = -1.0f;

	public bool bAgentBasedAssignment = false;

	float playerScaleForCrash = 0.5f;

	bool bTestingMGS = false;
	bool bTestingMGS2 = false;
	bool bTestingChung = false;
	bool bTestingMyScene1 = false;
	bool bTestingMyCrash = false;
	bool bJustTestCrashNow = false;

	float playerScaleForMyScene1 = 0.4f;
	float playerScaleForTestCase1 = 0.4f;
	float playerScaleForMGS2 = 0.5f;
	float playerScaleForChung = 0.5f;
	float playerScaleForMyCrash = 0.3f;
	float playerScaleForWastleLands2 = 0.5f;
	float playerScaleForCurrent;


	int PointToDebug = 147;
	public bool bDebugNow = false;
	bool bShowShadowEdges = true;
	void Start () 
	{
		//testFunc();
		//return;


		radius_enemy = ((CapsuleCollider)enemyPrefab.GetComponent<Collider>()).radius*((CapsuleCollider)enemyPrefab.GetComponent<Collider>()).transform.lossyScale.x;
		//Debug.Log ("radius_enemy = "+radius_enemy);

		spTemp = (GameObject)GameObject.Find ("StartPoint");
		allLineParent = GameObject.Find ("allLineParent") as GameObject;
		string[] sceneName = EditorApplication.currentScene.Split(char.Parse("/"));
		currSceneName = sceneName[sceneName.Length -1];
		globalPolygon = getObstacleEdges ();
		fileLastCaseExecutedFor = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\lastCaseExecutedFor1.txt";
		fileTimings = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\timingScene1.txt";
		filePoints = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\pointsScene1.txt";
		file_proveShadowAssisted = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\proveShadowAssisted1.txt";
		file_AgentBasedEachLevelFolder = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Desktop)+"\\AgentBasedEachLevel";

		if(currSceneName=="wastleLands2.unity") //USED
		{
			playerScaleForCurrent = playerScaleForWastleLands2;
		}
		else if(currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")//USED
		{
			playerScaleForCurrent = playerScaleForMyCrash;
		}
		else if(currSceneName=="myCrash.unity")
		{
			playerScaleForCurrent = playerScaleForMyCrash;
		}
		else if(currSceneName=="Crash.unity")
		{
			playerScaleForCurrent = playerScaleForMyCrash;
		}
		else if(currSceneName=="myScene1.unity")//USED
		{
			playerScaleForCurrent = playerScaleForMyScene1;
			m_step = 0.1f;
		}
		else if(currSceneName=="scene1.unity")
		{
			playerScaleForCurrent = playerScaleForMyScene1;
		}
		else if(currSceneName=="testCase1.unity")//USED
		{
			playerScaleForCurrent = playerScaleForTestCase1;
			m_step = 0.1f;//0.06f;
		}
		else if(currSceneName=="MGS2.unity")//USED
		{
			playerScaleForCurrent = playerScaleForMGS2;
		}
		else if(currSceneName=="chung.unity")//USED
		{
			playerScaleForCurrent = playerScaleForChung;
		}

		if(bMultiplePaths)
		{
			setUpMultiplePaths();
			return;
		}
		if(currSceneName=="wastleLands2.unity")
		{
			pathPoints = CommonWasteLands2.definePath ();
			m_stepDistance = CommonWasteLands2.getStepDistance();
			radius_enemy*=playerScaleForCurrent;
			m_step = 0.1f;
			/*if(bDebugNow)
			{
				Debug.Log("pathPoints = "+pathPoints.Count);
				setGlobalVars1();
				CalculateVisibilityForPath();
				int ptWhich = 50;
				int ptWhichCOunter=0;
				float standardMaxMovementTemp = speedEnemy*(m_stepDistance/speedPlayer);
				Debug.Log("standardMaxMovementTemp = "+standardMaxMovementTemp);
				foreach(Vector3 vect in h_mapPtToIndx.Keys)
				{
					if(ptWhichCOunter>=ptWhich && pointInShadow(vect,PointToDebug))
					{
						showPosOfPointEnemySized(vect,Color.green);
						foreach(Vector3 vectNeighbor in h_mapPtToIndx.Keys)
						{
							if(Vector3.Distance(vect,vectNeighbor)<=standardMaxMovementTemp)
							{
								showPosOfPointEnemySized(vectNeighbor,Color.red);
								//showPosOfPointRectangle(vect,Color.green);
							}
						}
						break;
					}
					ptWhichCOunter++;
				}
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}
				return;
			}*/
			
		}
		else if(currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			//pathPoints = CommonCrashShorter.definePath ();
			int selectPathIndx = 2;
			if(selectPathIndx==1)
			{
				GameObject sp2 = (GameObject)GameObject.Find ("StartPoint2");
				GameObject ep2 = (GameObject)GameObject.Find ("EndPoint2");
				(sp2.GetComponent<MeshRenderer>()).enabled = false;
				(ep2.GetComponent<MeshRenderer>()).enabled = false;
			}
			else if(selectPathIndx==2)
			{
				GameObject sp1 = (GameObject)GameObject.Find ("StartPoint1");
				GameObject ep1 = (GameObject)GameObject.Find ("EndPoint1");
				(sp1.GetComponent<MeshRenderer>()).enabled = false;
				(ep1.GetComponent<MeshRenderer>()).enabled = false;
			}
			pathPoints = CommonCrashShorter.definePath2 ();
			m_stepDistance = CommonCrashShorter.getStepDistance();
			radius_enemy*=playerScaleForCurrent;
			m_step = 0.055f;
			/*if(bDebugNow)
			{
				Debug.Log("pathPoints = "+pathPoints.Count);
				setGlobalVars1();
				CalculateVisibilityForPath();
				int ptWhich = 50;
				int ptWhichCOunter=0;
				float standardMaxMovementTemp = speedEnemy*(m_stepDistance/speedPlayer);
				Debug.Log("standardMaxMovementTemp = "+standardMaxMovementTemp);
				foreach(Vector3 vect in h_mapPtToIndx.Keys)
				{
					if(ptWhichCOunter>=ptWhich && pointInShadow(vect,PointToDebug))
					{
						showPosOfPointEnemySized(vect,Color.green);
						foreach(Vector3 vectNeighbor in h_mapPtToIndx.Keys)
						{
							if(Vector3.Distance(vect,vectNeighbor)<=standardMaxMovementTemp)
							{
								showPosOfPointEnemySized(vectNeighbor,Color.red);
								//showPosOfPointRectangle(vect,Color.green);
							}
						}
						break;
					}
					ptWhichCOunter++;
				}
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}
				return;
			}*/
			
		}
		else if(currSceneName=="myCrash.unity")
		{
			pathPoints = CommonCrash.definePath ();
			m_stepDistance = CommonCrash.getStepDistance();
			radius_enemy*=playerScaleForCurrent;
			m_step = 0.05f;
			/*if(bDebugNow)
			{
				Debug.Log("pathPoints = "+pathPoints.Count);
				setGlobalVars1();
				CalculateVisibilityForPath();
				int ptWhich = 100;
				int ptWhichCOunter=0;
				float standardMaxMovementTemp = speedEnemy*(m_stepDistance/speedPlayer);
				Debug.Log("standardMaxMovementTemp = "+standardMaxMovementTemp);
				foreach(Vector3 vect in h_mapPtToIndx.Keys)
				{
					if(ptWhichCOunter>=ptWhich && pointInShadow(vect,PointToDebug))
					{
						showPosOfPointEnemySized(vect,Color.green);
						foreach(Vector3 vectNeighbor in h_mapPtToIndx.Keys)
						{
							if(Vector3.Distance(vect,vectNeighbor)<=standardMaxMovementTemp)
							{
								showPosOfPointEnemySized(vectNeighbor,Color.red);
								//showPosOfPointRectangle(vect,Color.green);
							}
						}
						break;
					}
					ptWhichCOunter++;
				}
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}
				return;
			}*/

		}
		else if(currSceneName=="Crash.unity")
		{
			//Debug.Log("Before changing radius = "+radius_enemy);
			radius_enemy*=playerScaleForCurrent;
			//Debug.Log("After changing radius = "+radius_enemy);

			pathPoints = CommonCrash.definePath ();
			m_stepDistance = CommonCrash.getStepDistance();
			m_step = 0.3f;//m_stepDistance*playerScaleForCrash;
			Debug.Log("m_stepDistance = "+m_stepDistance);
			//***********************************************************************
			if(true)
			{
			int totalVertices = 0;
			totalVertices+=mapBG.edges.Count;
			int itr9=0;
			while(itr9<totalVertices)
			{
				GameObject allLineParentTemp = new GameObject();
				LineRenderer line1 = allLineParentTemp.AddComponent<LineRenderer> ();
				line1.material = mat;
				line1.SetWidth (0.3f, 0.3f);
				//line1.SetColors(Color.blue,Color.blue);
				line1.SetVertexCount (2);
				line1.SetPosition (0, mapBG.edges[itr9].vertex [0]);
				line1.SetPosition (1, mapBG.edges[itr9].vertex [1]);
				itr9++;
			}
			totalVertices=0;

			for(int i=0;i<globalPolygon.Count;i++)
			{
				totalVertices+=globalPolygon[i].edges.Count;
			}
			for(int i=0;i<globalPolygon.Count;i++)
			{
				foreach(Line l in globalPolygon[i].edges)
				{
					GameObject allLineParentTemp = new GameObject();
					LineRenderer line1 = allLineParentTemp.AddComponent<LineRenderer> ();
					line1.material = mat;
					line1.SetWidth (0.3f, 0.3f);
					line1.SetVertexCount (2);
					line1.SetPosition (0, l.vertex [0]);
					line1.SetPosition (1, l.vertex [1]);

				}
			}
				/*return;
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}

				setGlobalVars1();
				CalculateVisibilityForPath();
				int ptWhich = 900;
				int ptWhichCOunter=0;
				float standardMaxMovementTemp = speedEnemy*(m_stepDistance/speedPlayer);
				Debug.Log("standardMaxMovementTemp = "+standardMaxMovementTemp);
				foreach(Vector3 vect in h_mapPtToIndx.Keys)
				{
					if(ptWhichCOunter>=ptWhich && pointInShadow(vect,0))
					{
						showPosOfPointEnemySized(vect,Color.green);
						foreach(Vector3 vectNeighbor in h_mapPtToIndx.Keys)
						{
							if(Vector3.Distance(vect,vectNeighbor)<=standardMaxMovementTemp)
							{
								showPosOfPointEnemySized(vectNeighbor,Color.red);
								//showPosOfPointRectangle(vect,Color.green);
							}
						}
						break;
					}
					ptWhichCOunter++;
				}*/

				if(bJustTestCrashNow)
				{
					setGlobalVars1();
					CalculateVisibilityForPath();
					//
					int j1=0;
					for(float j=m_minX;j<m_maxX && j1<discretePtsX;j+=m_step)
					{
						int k1=0;
						for(float k=m_minZ;k<m_maxZ && k1<discretePtsZ;k+=m_step)
						{
							Vector3 pt = new Vector3(j,1,k);
							if(CheckIfInsidePolygon(pt) || !mapBG.PointInside(pt))
							{
								
								
								showPosOfPoint(pt,Color.green);
								
								
							}
							k1++;
						}
						j1++;
					}
					return;
					//
					foreach(Vector3 vect in pathPoints)
					{
						GameObject pathObj;
						pathObj = Instantiate(pathSphere, 
						                      vect, 
						                      pathSphere.transform.rotation) as GameObject;
					}

					playerObj = Instantiate(playerPrefab) as GameObject;

						Vector3 lscale1= playerObj.transform.localScale;
					lscale1.x*=playerScaleForCurrent;
					lscale1.y*=playerScaleForCurrent;
					lscale1.z*=playerScaleForCurrent;
						Renderer rend = playerObj.GetComponent<Renderer>();
						rend.transform.localScale = lscale1;
					
					
					playerObj.transform.position = pathPoints [0];
					return;
				}
			}
		}
		else if(currSceneName=="myScene1.unity")
		{
			pathPoints = CommonMyScene1.definePath ();
			m_stepDistance = CommonMyScene1.getStepDistance();
			radius_enemy*=playerScaleForCurrent;

		}
		else if(currSceneName=="scene1.unity")
		{
			pathPoints = CommonScene1.definePath ();
			m_stepDistance = CommonScene1.getStepDistance();
		}
		else if(currSceneName=="testCase1.unity")
		{
			pathPoints = CommonTestCase1.definePath ();
			m_stepDistance = CommonTestCase1.getStepDistance();
		}
		else if(currSceneName=="MGS2.unity")
		{
			//setGlobalVars1();//deleteNow
			pathPoints = CommonMGS2.definePath ();
			m_stepDistance = CommonMGS2.getStepDistance();
			Debug.Log("pathPoints.Count = "+pathPoints.Count);
			if(bTestingMGS2)
			{
				setGlobalVars1();
				CalculateVisibilityForPath();
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}
				return;
			}
		}
		else if(currSceneName=="MGS.unity")
		{
			pathPoints = CommonMGS.definePath ();
			m_stepDistance = CommonMGS.getStepDistance();
			Debug.Log("pathPoints.Count = "+pathPoints.Count);
			if(bTestingMGS)
			{
				setGlobalVars1();
				CalculateVisibilityForPath();
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}
				return;
			}
		}
		else if(currSceneName=="chung.unity")
		{
			pathPoints = CommonChung.definePath ();
			m_stepDistance = CommonChung.getStepDistance();
			Debug.Log("pathPoints.Count = "+pathPoints.Count);
			if(bTestingChung)
			{
				setGlobalVars1();
				CalculateVisibilityForPath();
				foreach(Vector3 vect in pathPoints)
				{
					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
					                      vect, 
					                      pathSphere.transform.rotation) as GameObject;
				}
				return;
			}
		}
		if(bDebugNow)
		{
			Debug.Log("pathPoints = "+pathPoints.Count);
			setGlobalVars1();
			CalculateVisibilityForPath();
			int ptWhich = 50;
			int ptWhichCOunter=0;
			float standardMaxMovementTemp = speedEnemy*(m_stepDistance/speedPlayer);
			Debug.Log("standardMaxMovementTemp = "+standardMaxMovementTemp);
			foreach(Vector3 vect in h_mapPtToIndx.Keys)
			{
				if(ptWhichCOunter>=ptWhich && pointInShadow(vect,PointToDebug))
				{
					showPosOfPointEnemySized(vect,Color.green);
					foreach(Vector3 vectNeighbor in h_mapPtToIndx.Keys)
					{
						if(Vector3.Distance(vect,vectNeighbor)<=standardMaxMovementTemp)
						{
							showPosOfPointEnemySized(vectNeighbor,Color.red);
							//showPosOfPointRectangle(vect,Color.green);
						}
					}
					break;
				}
				ptWhichCOunter++;
			}
			foreach(Vector3 vect in pathPoints)
			{
				GameObject pathObj;
				pathObj = Instantiate(pathSphere, 
				                      vect, 
				                      pathSphere.transform.rotation) as GameObject;
			}
			return;
		}
		if(bAgentBasedAssignment)
		{
			setGlobalVars1();
			CalculateVisibilityForPath();
			//agentBasedAssignment();
			agentBasedAssignmentFromEnd();
			return;
		}
		///////////////////////////True Case//////////////////////////////
		if(m_ExecuteTrueCase)
		{
			CalculateVisibilityForPath ();
			//executeTrueCase2();
			//executeTrueCase3();
			executeTrueCase4();
			//executeTrueCase5();
			//executeTrueCase6();
			return;
		}
		if(m_CalculateTrueCase)
		{
			//calculatePredictedPaths();
			calculatePredictedPathsNew();
			return;
		}
		if(m_DisplayOptimizedPaths)
		{
			displayOptimizedPaths();
			return;
		}
		if(m_ShowTrueCase)
		{
			displayPredictedPaths3();
			if(currSceneName=="Crash.unity")
			{

				int totalVertices = 0;
				totalVertices+=mapBG.edges.Count;
				int itr9=0;
				while(itr9<totalVertices)
				{
					GameObject allLineParentTemp = new GameObject();
					LineRenderer line1 = allLineParentTemp.AddComponent<LineRenderer> ();
					line1.material = mat;
					line1.SetWidth (0.3f, 0.3f);
					//line1.SetColors(Color.blue,Color.blue);
					line1.SetVertexCount (2);
					Vector3 vertexPt = new Vector3(mapBG.edges[itr9].vertex [0].x,2.0f,mapBG.edges[itr9].vertex [0].z);
					line1.SetPosition (0,vertexPt);
					vertexPt = new Vector3(mapBG.edges[itr9].vertex [1].x,2.0f,mapBG.edges[itr9].vertex [1].z);
					line1.SetPosition (1, vertexPt);
					itr9++;
				}
				totalVertices=0;
				
				for(int i=0;i<globalPolygon.Count;i++)
				{
					totalVertices+=globalPolygon[i].edges.Count;
				}
				for(int i=0;i<globalPolygon.Count;i++)
				{
					foreach(Line l in globalPolygon[i].edges)
					{
						GameObject allLineParentTemp = new GameObject();
						LineRenderer line1 = allLineParentTemp.AddComponent<LineRenderer> ();
						line1.material = mat;
						line1.SetWidth (0.3f, 0.3f);
						line1.SetVertexCount (2);
						Vector3 vertexPt = new Vector3(l.vertex [0].x,2.0f,l.vertex [0].z);
						line1.SetPosition (0, vertexPt);
						vertexPt = new Vector3(l.vertex [1].x,2.0f,l.vertex [1].z);
						line1.SetPosition (1, vertexPt);
						
					}
				}
				
			}



			/*foreach(Vector3 vect in pathPoints)
			{
				GameObject pathObj;
				pathObj = Instantiate(pathSphere, 
				                      vect, 
				                      pathSphere.transform.rotation) as GameObject;
			}*/
			return;
		}
		/////////////////////////////////////////////////////////
		if(!bShowJustVisibilityPoly)
		{
			foreach(Vector3 vect in pathPoints)
			{
				GameObject pathObj;
				pathObj = Instantiate(pathSphere, 
				                    vect, 
				                    pathSphere.transform.rotation) as GameObject;
			}
		}
		if (bDisplayAreas)
		{
			//readTimings();
			displayTimingAreas();
			return;
		}
		CalculateVisibilityForPath ();
		shadowMeshes = new List<GameObject>();
		playerObj = Instantiate(playerPrefab) as GameObject;



		Vector3 lscale= playerObj.transform.localScale;
		lscale.x*=playerScaleForCurrent;
		lscale.y*=playerScaleForCurrent;
		lscale.z*=playerScaleForCurrent;
		Renderer rend1 = playerObj.GetComponent<Renderer>();
		rend1.transform.localScale = lscale;
		


		playerObj.transform.position = pathPoints [0];
		/*foreach(Line l in ((Geometry)hVisiblePolyTable[pathPoints[0]]).edges)
		{
			l.DrawVector(allLineParent);
		}*/
		if(m_SetUpCase)
		{
			if(m_Greedy)
			{
				initializeForGreedyCase();
			}
			else if(m_NearMiss)
			{
				initializeForNearMissCase();
			}
			else if(m_ShadowEdgeAssisted)
			{
				initializeShadowEdgeAssisted();//ForCentroidCase();
			}
			return;
		}
		else if(m_ContinueCase)
		{
			readTimings();
			StreamReader sr = new StreamReader(fileLastCaseExecutedFor);


			List<char> sep = new List<char>();
			sep.Add(',');
			string str = sr.ReadLine();
			sr.Close();
			string[] strArr = str.Split(sep.ToArray());

			m_nCurrDiscretePtIndxX = int.Parse(strArr[0]);
			m_nCurrDiscretePtIndxZ = int.Parse(strArr[1]);


			Debug.Log ("m_nCurrDiscretePtIndxX read = " + m_nCurrDiscretePtIndxX);
			Debug.Log ("m_nCurrDiscretePtIndxZ read = " + m_nCurrDiscretePtIndxZ);

			Vector3 tempVec = (Vector3)h_mapIndxToPt[new Vector2(m_nCurrDiscretePtIndxX,m_nCurrDiscretePtIndxZ)];
			Debug.Log("placeEnemyNearMissAt = "+tempVec);

			if(m_Greedy)
			{
				placeEnemyGreedyAt(tempVec);
			}
			else if(m_NearMiss)
			{
				placeEnemyNearMissAt(tempVec);
			}
			else if(m_ShadowEdgeAssisted)
			{
				placeEnemyShadowAssistedAt(tempVec);
			}
			resetCase ();
			Debug.Log("After resetCase: m_nCurrDiscretePtIndxX = "+m_nCurrDiscretePtIndxX);
			Debug.Log("After resetCase: m_nCurrDiscretePtIndxZ = "+m_nCurrDiscretePtIndxZ);
			return;
		}
		////////////For Single run//////////////
		setUpEnemyInitialPos ();
	}

	private string createSaveDataDir (string dataPath)
	{
		string dirName = Path.Combine(dataPath ,System.DateTime.Now.Day
		                              +"th_Of_"+System.DateTime.Now.Month
		                              +"_Year_"+System.DateTime.Now.Year
		                              +"_Time_"+System.DateTime.Now.Hour
		                              +"_"+System.DateTime.Now.Minute
		                              +"_"+System.DateTime.Now.Second
		                              +"_PredictedPathData");
		System.IO.Directory.CreateDirectory(dirName);
		return dirName;
	}

	void resetCase()
	{
		if (m_nCurrDiscretePtIndxZ >= discretePtsZ)
		{
			Debug.Break();
			writeTimings();
			return;
		}
		int skipPts = 1;
		while(true)
		{
			Vector3 tempVec = (Vector3)h_mapIndxToPt[new Vector2(m_nCurrDiscretePtIndxX,m_nCurrDiscretePtIndxZ)];
			//Vector3 tempVec = new Vector3 (m_nCurrDiscretePtIndxX, 1, m_nCurrDiscretePtIndxZ);
			if(pointInShadow(tempVec,0) && pointsArray[m_nCurrDiscretePtIndxX,m_nCurrDiscretePtIndxZ]==0)
			{
				//Debug.Log(tempVec+" in the shadow. Selecting this");
				//Assumption:Only one enemy at a time
				playerObj.transform.position = pathPoints [0];
				nextPlayerPath = 1;



				if(m_Greedy)
				{
					m_enemyGreedyList[0].enemyObj.transform.position = tempVec;
					m_enemyGreedyList[0].vNextPos.RemoveRange(0,m_enemyGreedyList[0].vNextPos.Count);
					m_enemyGreedyList[0].vNextPos.Add (findNextPosEnemyGreedy(m_enemyGreedyList[0].enemyObj,m_enemyGreedyList[0].enemyObj.transform.position));
					m_enemyGreedyList[0].bCaught = false;
				}
				else if(m_NearMiss)
				{
					m_enemyNearMissList[0].enemyObj.transform.position = tempVec;
					m_enemyNearMissList[0].vNextPos.RemoveRange(0,m_enemyNearMissList[0].vNextPos.Count);
					m_enemyNearMissList[0].vNextPos.Add(findNextPosEnemyNearMiss2(m_enemyNearMissList[0].enemyObj,m_enemyNearMissList[0].enemyObj.transform.position));
					m_enemyNearMissList[0].bCaught = false;
				}
				else if(m_ShadowEdgeAssisted)
				{
					m_enemyShadowAssistedList[0].enemyObj.transform.position = tempVec;
					m_enemyShadowAssistedList[0].vNextPos.RemoveRange(0,m_enemyShadowAssistedList[0].vNextPos.Count);
					m_enemyShadowAssistedList[0].vNextPos.Add(findNextPosEnemyShadowAssisted(m_enemyShadowAssistedList[0].enemyObj,m_enemyShadowAssistedList[0].enemyObj.transform.position));
					m_enemyShadowAssistedList[0].bCaught = false;
				}
				Debug.Log("Selected At ("+m_nCurrDiscretePtIndxX+" , "+m_nCurrDiscretePtIndxZ+")"+"Vector3 = "+tempVec);
				break;


			}
			else
			{
				//Debug.Log("Reset Case. "+tempVec+" not in shadow. At ("+m_nCurrDiscretePtIndxX+" , "+m_nCurrDiscretePtIndxZ+")");
			}
			m_nCurrDiscretePtIndxX+=skipPts;
			if (m_nCurrDiscretePtIndxX >= discretePtsX)
			{
				m_nCurrDiscretePtIndxX=0;
				m_nCurrDiscretePtIndxZ+=skipPts;
			}

			if (m_nCurrDiscretePtIndxZ >= discretePtsZ)
			{
				Debug.Break();
				writeTimings();
				return;
			}
		}

		writeTimings ();
		currRunTimeEnemny = Time.time;
		currRunPointsEnemy = 0.0f;
	}
	public Material matGreen;
	//Distance moved by player on each update;
	float distBtwPlayerMovements = -1.0f;
	bool bSlowShadowsDown = false;
	int setTimerTemp = 0;
	bool bShowJustVisibilityPoly = false;
	int bShowJustVisibilityPolyForIndex = 53;

	int nextPosIndx9=0;
	List<GameObject> listShows = new List<GameObject>();

	private bool enemyCaught(Vector3 currPos)
	{
		if (pointInShadow (currPos, nextPlayerPath - 1))
			return false;
		return true;
	}
	private float distanceBwPtAndLine(Vector3 pt,Line currEdge)
	{
		bool validLine = false;
		float y2 = currEdge.vertex[1].z;
		float y1 = currEdge.vertex[0].z;
		float x2 = currEdge.vertex[1].x;
		float x1 = currEdge.vertex[0].x;
		float x3 = pt.x;
		float y3 = pt.z;

		float k = ((y2 - y1) * (x3 - x1) - (x2 - x1) * (y3 - y1)) / ((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
		float x4 = x3 - k * (y2-y1);
		float y4 = y3 + k * (x2-x1);
		Vector3 perpdicularPt = new Vector3 (x4, pt.y, y4);
		if(currEdge.PointOnLine(perpdicularPt))
		{
			validLine = true;

		}
		/*if((pt.x >=currEdge.vertex[0].x && pt.x <=currEdge.vertex[1].x) ||   
		   (pt.x >=currEdge.vertex[1].x && pt.x <=currEdge.vertex[0].x) || 
		   (pt.z >=currEdge.vertex[1].z && pt.z <=currEdge.vertex[0].z) || 
		   (pt.z >=currEdge.vertex[0].z && pt.z <=currEdge.vertex[1].z))
		{
			validLine = true;
		}*/
		/*if(currEdge.vertex[0].z == currEdge.vertex[1].z)
		{
			if((pt.x - currEdge.vertex[0].x)*(currEdge.vertex[1].x-pt.x)>0)
			{
				validLine = true;
			}
		}
		else if(currEdge.vertex[0].x == currEdge.vertex[1].x)
		{
			if((pt.z - currEdge.vertex[0].z)*(currEdge.vertex[1].z-pt.z)>0)
			{
				validLine = true;
			}
		}
		else if((pt.x - currEdge.vertex[0].x)*(currEdge.vertex[1].x-pt.x)>0 && (pt.z - currEdge.vertex[0].z)*(currEdge.vertex[1].z-pt.z)>0)
		{
			validLine = true;
		}*/

		//currEdge.DrawVector(allLineParent);
		if(!validLine)
		{
			//return -1.0f;
			if(Vector3.Distance(pt,currEdge.vertex[0])<Vector3.Distance(pt,currEdge.vertex[1]))
			{
				return Vector3.Distance(pt,currEdge.vertex[0]);
			}
			else
			{
				return Vector3.Distance(pt,currEdge.vertex[1]);
			}

			//return -1.0f;
		}

		float minDist = Mathf.Abs((y2-y1)*pt.x - (x2-x1)*pt.z + x2*y1 - y2*x1);
		minDist = minDist/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
		return minDist;
	}



	private float distanceBwPtAndLine4(Vector3 pt,Line currEdge,List<Line> allShadowLines)
	{
		float radiusMax = 5000f;
		float angleVar = 0f;
		Vector3 vecSel = new Vector3 ();
		vecSel.y = 1f;
		Line shadowEdgeCurrent = currEdge;

		angleVar = 0f;
		float minProbableDist = 10000f;
		while(angleVar<360)
		{
			vecSel.x = pt.x + radiusMax*Mathf.Cos(angleVar* Mathf.Deg2Rad);
			vecSel.z = pt.z + radiusMax*Mathf.Sin(angleVar* Mathf.Deg2Rad);
			Line l = new Line(pt,vecSel);
			if(l.LineIntersectMuntacEndPt(shadowEdgeCurrent)!=0)
			{
				Vector3 intsctPoint = l.GetIntersectionPoint(shadowEdgeCurrent);
				Line testLine = new Line(pt,intsctPoint);
				//now check if testLine and any other shadow edge intersect; if not then valid edge and break;
				bool bDidNotInteresct = true;
				foreach(Line l2 in allShadowLines)
				{
					if(l2.Equals(shadowEdgeCurrent))
						continue;
					if(l2.LineIntersectMuntacEndPt(testLine)!=0)
					{
						/*Vector3 intsctPt = l2.GetIntersectionPoint(testLine);
						if(VectorApprox2(intsctPt,l2.vertex[0]) || VectorApprox2(intsctPt,l2.vertex[1]))
						{
							continue;
						}*/
						bDidNotInteresct = false;
						break;
						//not valid
					}
				}
				//
				if(bDidNotInteresct)
				{
					float probableDist = testLine.LengthOfLine();
					if(minProbableDist>probableDist)
					{
						minProbableDist = probableDist;
					}
				}
				//
				
			}
			angleVar+=1f;
		}

		//Adding vertices too
		float probableDist1 = Vector3.Distance(shadowEdgeCurrent.vertex[0],pt);
		if(minProbableDist>probableDist1)
		{
			minProbableDist = probableDist1;

		}
		probableDist1 = Vector3.Distance(shadowEdgeCurrent.vertex[1],pt);
		if(minProbableDist>probableDist1)
		{
			minProbableDist = probableDist1;

		}
		
		return minProbableDist;
		
	}


	private float distanceBwPtAndLine3(Vector3 pt,Line currEdge)
	{
		bool validLine = false;
		float y2 = currEdge.vertex[1].z;
		float y1 = currEdge.vertex[0].z;
		float x2 = currEdge.vertex[1].x;
		float x1 = currEdge.vertex[0].x;
		float x3 = pt.x;
		float y3 = pt.z;
		
		float k = ((y2 - y1) * (x3 - x1) - (x2 - x1) * (y3 - y1)) / ((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
		float x4 = x3 - k * (y2-y1);
		float y4 = y3 + k * (x2-x1);
		Vector3 perpdicularPt = new Vector3 (x4, pt.y, y4);


		//Commented out as testing just mid points of valid edge
		if(!currEdge.PointOnLine(perpdicularPt))//
		{
			return Vector3.Distance(pt,currEdge.MidPoint());//should aim for a more central point
		}
		
		
		float minDist = Mathf.Abs((y2-y1)*pt.x - (x2-x1)*pt.z + x2*y1 - y2*x1);
		minDist = minDist/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
		return minDist;
	}

	/// <summary>
	/// </summary>
	/// <returns>The without clamp.</returns>
	/// <param name="A">A.</param>
	/// <param name="B">B.</param>
	/// <param name="t">T.</param>
	private float distanceBwPtAndLine2(Vector3 pt,Line currEdge,List<Line> allShadowLines)
	{
		bool validLine = false;
		float y2 = currEdge.vertex[1].z;
		float y1 = currEdge.vertex[0].z;
		float x2 = currEdge.vertex[1].x;
		float x1 = currEdge.vertex[0].x;
		float x3 = pt.x;
		float y3 = pt.z;
		
		float k = ((y2 - y1) * (x3 - x1) - (x2 - x1) * (y3 - y1)) / ((y2 - y1) * (y2 - y1) + (x2 - x1) * (x2 - x1));
		float x4 = x3 - k * (y2-y1);
		float y4 = y3 + k * (x2-x1);
		Vector3 perpdicularPt = new Vector3 (x4, pt.y, y4);
		if(currEdge.PointOnLine(perpdicularPt))
		{
			validLine = true;	
		}

		if((x3>=x1 && x3<=x2) ||   
		   (x3>=x2 && x3<=x1) || 
		   (y3>=y2 && y3<=y1) || 
		   (y3>=y1 && y3<=y2))
		{
			validLine = true;
		}

		if(!validLine)
		{
			return -1.0f;
		}

		//Another test. Do not cut another shadow edge.
		Line testLine = new Line (pt, perpdicularPt);
		foreach(Line l in allShadowLines)
		{
			if(l.Equals(currEdge))
				continue;
			if(l.LineIntersectMuntacEndPt(testLine)!=0)
			{
				validLine = false;
				return -1.0f;
			}
			Vector3 intsctPt = l.GetIntersectionPoint(testLine);
		}


		//Commented out as testing just mid points of valid edge
		if(!currEdge.PointOnLine(perpdicularPt))//
		{
			return Vector3.Distance(pt,currEdge.MidPoint());//should aim for a more central point
			/*if(Vector3.Distance(pt,currEdge.vertex[0])<Vector3.Distance(pt,currEdge.vertex[1]))
			{
				return Vector3.Distance(pt,currEdge.vertex[0]);
			}
			else
			{
				return Vector3.Distance(pt,currEdge.vertex[1]);
			}*/
		}


		float minDist = Mathf.Abs((y2-y1)*pt.x - (x2-x1)*pt.z + x2*y1 - y2*x1);
		minDist = minDist/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
		return minDist;
	}







	Vector3 LerpWithoutClamp(Vector3 A, Vector3 B, float t)
	{
		return A + (B-A)*t;
	}

	Line findPositionFurthestToNearestEdge(Vector3 pt,List<Line> allShadowLines)
	{
		float perpendicularDist;
		float radiusMax = 5000f;
		float angleVar = 0f;
		Vector3 vecSel = new Vector3 ();
		vecSel.y = 1f;
		Hashtable distanceLineTable = new Hashtable ();
		foreach(Line shadowEdgeCurrent in allShadowLines)
		{
			angleVar = 0f;
			float minProbableDist = 10000f;
			while(angleVar<360)
			{
				vecSel.x = pt.x + radiusMax*Mathf.Cos(angleVar* Mathf.Deg2Rad);
				vecSel.z = pt.z + radiusMax*Mathf.Sin(angleVar* Mathf.Deg2Rad);
				Line l = new Line(pt,vecSel);
				if(l.LineIntersectMuntacEndPt(shadowEdgeCurrent)!=0)
				{
					Vector3 intsctPoint = l.GetIntersectionPoint(shadowEdgeCurrent);
					Line testLine = new Line(pt,intsctPoint);
					//now check if testLine and any other shadow edge intersect; if not then valid edge and break;
					bool bDidNotInteresct = true;
					foreach(Line l2 in allShadowLines)
					{
						if(l2.Equals(shadowEdgeCurrent))
							continue;
						if(l2.LineIntersectMuntacEndPt(testLine)!=0)
						{
							bDidNotInteresct = false;
							break;
							//not valid
						}
						//Vector3 intsctPt = l2.GetIntersectionPoint(testLine);
					}
					//
					if(bDidNotInteresct)
					{
						float probableDist = testLine.LengthOfLine();//distanceBwPtAndLine3(pt,shadowEdgeCurrent);
						if(minProbableDist>probableDist)
						{
							minProbableDist = probableDist;
							if(!distanceLineTable.ContainsKey(shadowEdgeCurrent))
							{
								distanceLineTable.Add(shadowEdgeCurrent,minProbableDist);
							}
							else
							{
								distanceLineTable[shadowEdgeCurrent] = minProbableDist;
							}
						}
						//distanceLineTable.Add(shadowEdgeCurrent,probableDist);
						//break;
					}
					//
					
				}
				angleVar+=1f;
			}
		}
		float minVal = 100000f;
		Line selLine = null;
		foreach(Line shadowEdgeKey in distanceLineTable.Keys)
		{
			float val = (float)distanceLineTable[shadowEdgeKey];
			if(val<minVal)
			{
				minVal = val;
				selLine = shadowEdgeKey;
			}
		}
		return selLine;
	}







	List<Line> findBothLines(Vector3 pt,List<Line> allShadowLines)
	{
		float perpendicularDist;
		float radiusMax = 5000f;
		float angleVar = 0f;
		Vector3 vecSel = new Vector3 ();
		vecSel.y = 1f;
		Hashtable distanceLineTable = new Hashtable ();
		foreach(Line shadowEdgeCurrent in allShadowLines)
		{
			angleVar = 0f;
			float minProbableDist = 10000f;
			while(angleVar<360)
			{
				vecSel.x = pt.x + radiusMax*Mathf.Cos(angleVar* Mathf.Deg2Rad);
				vecSel.z = pt.z + radiusMax*Mathf.Sin(angleVar* Mathf.Deg2Rad);
				Line l = new Line(pt,vecSel);
				if(l.LineIntersectMuntacEndPt(shadowEdgeCurrent)!=0)
				{
					Vector3 intsctPoint = l.GetIntersectionPoint(shadowEdgeCurrent);
					Line testLine = new Line(pt,intsctPoint);
					//now check if testLine and any other shadow edge intersect; if not then valid edge and break;
					bool bDidNotInteresct = true;
					foreach(Line l2 in allShadowLines)
					{
						if(l2.Equals(shadowEdgeCurrent))
							continue;
						if(l2.LineIntersectMuntacEndPt(testLine)!=0)
						{
							//if(intersects, checked if Not as Vertex point)
							Vector3 intsctPt = l2.GetIntersectionPoint(testLine);
							if(VectorApprox2(intsctPt,l2.vertex[0]) || VectorApprox2(intsctPt,l2.vertex[1]))
							{
								continue;
							}
							bDidNotInteresct = false;
							break;
							//not valid
						}

					}
					//
					if(bDidNotInteresct)
					{
						float probableDist = testLine.LengthOfLine();//distanceBwPtAndLine3(pt,shadowEdgeCurrent);
						if(minProbableDist>probableDist)
						{
							minProbableDist = probableDist;
							if(!distanceLineTable.ContainsKey(shadowEdgeCurrent))
							{
								distanceLineTable.Add(shadowEdgeCurrent,minProbableDist);
							}
							else
							{
								distanceLineTable[shadowEdgeCurrent] = minProbableDist;
							}
						}
						//distanceLineTable.Add(shadowEdgeCurrent,probableDist);
						//break;
					}
					//
					
				}
				//angleVar+=10f;
				angleVar+=1f;
			}
			//Adding vertices too; wrong as invalid vertices added too
			/*float probableDist1 = Vector3.Distance(shadowEdgeCurrent.vertex[0],pt);
			if(minProbableDist>probableDist1)
			{
				minProbableDist = probableDist1;
				if(!distanceLineTable.ContainsKey(shadowEdgeCurrent))
				{
					distanceLineTable.Add(shadowEdgeCurrent,minProbableDist);
				}
				else
				{
					distanceLineTable[shadowEdgeCurrent] = minProbableDist;
				}
			}
			probableDist1 = Vector3.Distance(shadowEdgeCurrent.vertex[1],pt);
			if(minProbableDist>probableDist1)
			{
				minProbableDist = probableDist1;
				if(!distanceLineTable.ContainsKey(shadowEdgeCurrent))
				{
					distanceLineTable.Add(shadowEdgeCurrent,minProbableDist);
				}
				else
				{
					distanceLineTable[shadowEdgeCurrent] = minProbableDist;
				}
			}*/
		}



		List<Line> bothLines = new List<Line> ();


		float minVal = 100000f;
		Line selLine1 = null;
		foreach(Line shadowEdgeKey in distanceLineTable.Keys)
		{
			float val = (float)distanceLineTable[shadowEdgeKey];
			if(val<minVal)
			{
				minVal = val;
				selLine1 = shadowEdgeKey;
			}
		}
		bothLines.Add(selLine1);




		float maxVal = -1000f;
		Line selLine2 = null;
		foreach(Line shadowEdgeKey in distanceLineTable.Keys)
		{
			float val = (float)distanceLineTable[shadowEdgeKey];
			if(val>maxVal)
			{
				maxVal = val;
				selLine2 = shadowEdgeKey;
			}
		}
		bothLines.Add(selLine2);
		return bothLines;




	}









	/// <summary>
	/// ///////////
	/// </summary>
	/// <returns>The position nearest to furthest edge.</returns>
	/// <param name="pt">Point.</param>
	/// <param name="allShadowLines">All shadow lines.</param>
	Line findPositionNearestToFurthestEdge(Vector3 pt,List<Line> allShadowLines)
	{
		float perpendicularDist;
		float radiusMax = 5000f;
		float angleVar = 0f;
		Vector3 vecSel = new Vector3 ();
		vecSel.y = 1f;
		Hashtable distanceLineTable = new Hashtable ();
		foreach(Line shadowEdgeCurrent in allShadowLines)
		{
			angleVar = 0f;
			float minProbableDist = 10000f;
			while(angleVar<360)
			{
				vecSel.x = pt.x + radiusMax*Mathf.Cos(angleVar* Mathf.Deg2Rad);
				vecSel.z = pt.z + radiusMax*Mathf.Sin(angleVar* Mathf.Deg2Rad);
				Line l = new Line(pt,vecSel);
				if(l.LineIntersectMuntacEndPt(shadowEdgeCurrent)!=0)
				{
					Vector3 intsctPoint = l.GetIntersectionPoint(shadowEdgeCurrent);
					Line testLine = new Line(pt,intsctPoint);
					//now check if testLine and any other shadow edge intersect; if not then valid edge and break;
					bool bDidNotInteresct = true;
					foreach(Line l2 in allShadowLines)
					{
						if(l2.Equals(shadowEdgeCurrent))
							continue;
						if(l2.LineIntersectMuntacEndPt(testLine)!=0)
						{
							bDidNotInteresct = false;
							break;
							//not valid
						}
						//Vector3 intsctPt = l2.GetIntersectionPoint(testLine);
					}
					//
					if(bDidNotInteresct)
					{
						float probableDist = testLine.LengthOfLine();//distanceBwPtAndLine3(pt,shadowEdgeCurrent);
						if(minProbableDist>probableDist)
						{
							minProbableDist = probableDist;
							if(!distanceLineTable.ContainsKey(shadowEdgeCurrent))
							{
								distanceLineTable.Add(shadowEdgeCurrent,minProbableDist);
							}
							else
							{
								distanceLineTable[shadowEdgeCurrent] = minProbableDist;
							}
						}
						//distanceLineTable.Add(shadowEdgeCurrent,probableDist);
						//break;
					}
					//

				}
				angleVar+=1f;
			}
		}
		float maxVal = -1000f;
		Line selLine = null;
		foreach(Line shadowEdgeKey in distanceLineTable.Keys)
		{
			float val = (float)distanceLineTable[shadowEdgeKey];
			if(val>maxVal)
			{
				maxVal = val;
				selLine = shadowEdgeKey;
			}
		}
		return selLine;
	}
	/// <summary>
	/// //////////////////////////////////////////////////////////////////
	/// </summary>
	/// <returns>The next position enemy shadow assisted old.</returns>
	/// <param name="enemyObj">Enemy object.</param>
	private static int nearestWeight = 3;
	private static int furthestWeight = 1;
	private static int nearestWeightCounter = nearestWeight;
	private static int furthestWeightCounter = furthestWeight;
	private Vector3 findNextPosEnemyShadowAssisted(GameObject enemyObj,Vector3 currPosEnemy)
	{
		//Debug.Log ("findNextPosEnemyShadowAssisted ");
		Vector3 vecSel = currPosEnemy;//enemyObj.transform.position;
		float timePlayer = Vector3.Distance(pathPoints[nextPlayerPath],pathPoints[nextPlayerPath-1])/speedPlayer;
		//float timePlayer = distBtwPlayerMovements/speedPlayer;//commented out due to bigger jumps
		float radiusMovement2 = speedEnemy*timePlayer;
		int currPlayerPathPoint = nextPlayerPath-1;
		if(!pointInShadow(vecSel,currPlayerPathPoint))
		{
			return currPosEnemy;//enemyObj.transform.position;
		}
		Geometry currShadowPolygon = null;
		/*Geometry currShadowPolygon = findCurrentShadowPolygon(vecSel,currPlayerPathPoint);
		if(currShadowPolygon==null)
		{
			Debug.Log("ERROR. Cannot find the current shadow polygon");
			return enemyObj.transform.position;
		}*/
		List<Vector3> probablePointsInShadow = new List<Vector3> ();
		int angleVar=0;
		int angleVarStep = skipAngleShadowAssistedGlobal;
		while(true)
		{
			vecSel = new Vector3();
			vecSel.x = currPosEnemy.x + radiusMovement2*Mathf.Cos(angleVar* Mathf.Deg2Rad);
			vecSel.y = currPosEnemy.y;
			vecSel.z = currPosEnemy.z + radiusMovement2*Mathf.Sin(angleVar* Mathf.Deg2Rad);
			if(pointInShadow(vecSel,currPlayerPathPoint) && CheckStraightLineVisibility(vecSel,currPosEnemy))
			{
				probablePointsInShadow.Add(vecSel);
			}
			angleVar+=angleVarStep;
			if(angleVar==360)
				break;
		}
		if(probablePointsInShadow.Count==0)
		{
			Debug.Log("ERROR. probablePointsInShadow.Count==0");
			return currPosEnemy;//enemyObj.transform.position;
		}
		else if(probablePointsInShadow.Count==1)
		{
			return probablePointsInShadow[0];
		}
		//probablePointsInShadow.Add(enemyObj.transform.position);

		//bool bNewAlgo = false;
		//bool bGoToFurthestShadowEdge = false;
		//bool bGoFromNearestShadowEdge = false;
		//List of all vertices which are visible.
		//List<Vector3> listAllVertices = new List<Vector3> ();
		/*if(!bNewAlgo && !bGoToFurthestShadowEdge)
		{

			for(int i=0;i<currShadowPolygon.edges.Count;i++)
			{
				if(!listAllVertices.Contains(currShadowPolygon.edges[i].vertex[0]))
				{
					listAllVertices.Add(currShadowPolygon.edges[i].vertex[0]);
				}
				if(!listAllVertices.Contains(currShadowPolygon.edges[i].vertex[1]))
				{
					listAllVertices.Add(currShadowPolygon.edges[i].vertex[1]);
				}
				if(!listAllVertices.Contains(currShadowPolygon.edges[i].MidPoint()))
				{
					listAllVertices.Add(currShadowPolygon.edges[i].MidPoint());
				}
			}
		}*/
		/////////////////////////////////////////////////////////
		if(true)
		{

			List<float> vals1 = new List<float>();
			List<float> vals2 = new List<float>();
			List<float> vals3 = new List<float>();
			List<float> vals4 = new List<float>();





			List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [currPlayerPathPoint]];
			List<Line> allShadowLines = new List<Line>();
			foreach(Geometry shadowCurr in shadowPolyTemp)
			{
				allShadowLines.AddRange(shadowCurr.edges);
			}
			Vector3 selectedPt = new Vector3();



			//float beforeTime = Time.realtimeSinceStartup;
			List<Line> bothLines = findBothLines(currPosEnemy,allShadowLines);
			//float afterTime = Time.realtimeSinceStartup;
			//afterTime = afterTime - beforeTime;
			//Debug.Log("Total time for findBothLines = "+afterTime);



			Line furthestEdge = bothLines[1];//findPositionNearestToFurthestEdge(enemyObj.transform.position,allShadowLines);
			
			//
			float minFurthestDistance = 0f;//distanceBwPtAndLine4(probablePointsInShadow[0],furthestEdge,allShadowLines);
			//selectedPt = probablePointsInShadow[0];
			for(int probPts=0;probPts<probablePointsInShadow.Count;probPts++)
			{
				float furthestDistance = distanceBwPtAndLine4(probablePointsInShadow[probPts],furthestEdge,allShadowLines);
				vals1.Add(furthestDistance);
			}
			float vals1Temp = vals1[0];
			for(int i=1;i<vals1.Count;i++)
			{
				if(vals1Temp<vals1[i])
				{
					vals1Temp = vals1[i];
				}
			}
			for(int i=0;i<vals1.Count;i++)
			{
				vals1[i]=Mathf.Abs(vals1[i]-vals1Temp);
			}





			Line nearestEdge = bothLines[0];//findPositionFurthestToNearestEdge(enemyObj.transform.position,allShadowLines);
			
			//
			float maxNearestDistance = 0f;//distanceBwPtAndLine4(probablePointsInShadow[0],nearestEdge,allShadowLines);
			//selectedPt = probablePointsInShadow[0];
			for(int probPts=0;probPts<probablePointsInShadow.Count;probPts++)
			{
				float nearestDistance = distanceBwPtAndLine4(probablePointsInShadow[probPts],nearestEdge,allShadowLines);
				vals2.Add(nearestDistance);
			}

			///////////////////////////////
			Vector3 nearestVertex = findNearestVertex(currPosEnemy,allShadowLines);
			maxNearestDistance = 0f;
			for(int probPts=0;probPts<probablePointsInShadow.Count;probPts++)
			{
				float nearestDistance = Vector3.Distance(nearestVertex,probablePointsInShadow[probPts]);
				vals4.Add(nearestDistance);
			}
			//////////////////////////////


			for(int i=0;i<vals1.Count;i++)
			{
				//vals3.Add (vals1[i]*0.5f+vals2[i]+vals4[i]*0.2f);
				//vals3.Add (vals1[i]*0.3f+vals2[i]+vals4[i]*1.0f);
				vals3.Add (vals1[i]*0.5f+vals2[i]+vals4[i]*0.2f);
			}

			//largest value
			float maxValTemp = vals3[0];
			int maxIndexTemp = 0;
			for(int i=1;i<vals3.Count;i++)
			{
				if(maxValTemp<vals3[i])
				{
					maxValTemp = vals3[i];
					maxIndexTemp = i;
				}
			}

			return probablePointsInShadow[maxIndexTemp];
		}
		/// /////////////////////////////////////////////////////
		//
		/*if(nearestWeightCounter>0)
		{
			bGoFromNearestShadowEdge = true;
			nearestWeightCounter--;
		}
		else if(furthestWeightCounter>0)
		{
			bGoToFurthestShadowEdge = true;
			furthestWeightCounter--;
		}
		if(furthestWeightCounter==0 && nearestWeightCounter==0)
		{
			furthestWeightCounter = furthestWeight;
			nearestWeightCounter = nearestWeight;
		}
		//
		if(bGoToFurthestShadowEdge)
		{
			List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [currPlayerPathPoint]];
			List<Line> allShadowLines = new List<Line>();
			foreach(Geometry shadowCurr in shadowPolyTemp)
			{
				allShadowLines.AddRange(shadowCurr.edges);
			}
			Vector3 selectedPt = new Vector3();
			//float minPerpendicularDist = 100000f;
			Line furthestEdge = findPositionNearestToFurthestEdge(enemyObj.transform.position,allShadowLines);

			//
			float minFurthestDistance = distanceBwPtAndLine4(probablePointsInShadow[0],furthestEdge,allShadowLines);
			selectedPt = probablePointsInShadow[0];
			for(int probPts=1;probPts<probablePointsInShadow.Count;probPts++)
			{
				float furthestDistance = distanceBwPtAndLine4(probablePointsInShadow[probPts],furthestEdge,allShadowLines);
				if(minFurthestDistance>furthestDistance)
				{
					minFurthestDistance = furthestDistance;
					selectedPt = probablePointsInShadow[probPts];
				}
			}

			return selectedPt;
		}
		else if(bGoFromNearestShadowEdge)
		{
			List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [currPlayerPathPoint]];
			List<Line> allShadowLines = new List<Line>();
			foreach(Geometry shadowCurr in shadowPolyTemp)
			{
				allShadowLines.AddRange(shadowCurr.edges);
			}
			Vector3 selectedPt = new Vector3();
			//float minPerpendicularDist = 100000f;
			Line nearestEdge = findPositionFurthestToNearestEdge(enemyObj.transform.position,allShadowLines);
			
			//
			float maxNearestDistance = distanceBwPtAndLine4(probablePointsInShadow[0],nearestEdge,allShadowLines);
			selectedPt = probablePointsInShadow[0];
			for(int probPts=1;probPts<probablePointsInShadow.Count;probPts++)
			{
				float nearestDistance = distanceBwPtAndLine4(probablePointsInShadow[probPts],nearestEdge,allShadowLines);
				if(maxNearestDistance<nearestDistance)
				{
					maxNearestDistance = nearestDistance;
					selectedPt = probablePointsInShadow[probPts];
				}
			}
			return selectedPt;
		}
		//
		if(bNewAlgo)
		{
			List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [currPlayerPathPoint]];
			List<Line> allShadowLines = new List<Line>();
			foreach(Geometry shadowCurr in shadowPolyTemp)
			{
				allShadowLines.AddRange(shadowCurr.edges);
			}
			Vector3 selectedPt = new Vector3();
			float minDiffDist = 100000.0f;
			foreach(Vector3 pt in probablePointsInShadow)
			{
				List<float> perpendicularDistList = new List<float>();
				for(int i=0;i<allShadowLines.Count;i++)
				{
					float perpendicularDist = distanceBwPtAndLine2(pt,allShadowLines[i],allShadowLines);
					if(perpendicularDist>=0f)
					{
						perpendicularDistList.Add(perpendicularDist);
					}
				}

				float numenator = 0f;
				foreach(float f in perpendicularDistList)
				{
					numenator+=f;
				}
				float meanDistance = numenator/perpendicularDistList.Count;
				float diffDist = 0.0f;
				foreach(float f in perpendicularDistList)
				{
					diffDist+=Mathf.Abs(f-meanDistance);
				}
				diffDist = diffDist/perpendicularDistList.Count;
				if(minDiffDist>diffDist)
				{
					minDiffDist = diffDist;
					selectedPt = pt;
				}

			}
			return selectedPt;
		}






		///////////Old Code/////////
		float overallMaxDist = -1.0f;
		int overallMaxDistIndex = -1;
		//Min distance from edges of shadow polygon
		List<float> probablePointsMinDist = new List<float> ();
		foreach(Vector3 pt in probablePointsInShadow)
		{
			float minDist = distanceBwPtAndLine(pt,currShadowPolygon.edges[0]);
			int iActual = 1;
			while(minDist<0.0f)
			{
				minDist = distanceBwPtAndLine(pt,currShadowPolygon.edges[iActual]);
				iActual++;
			}
			for(int i=iActual;i<currShadowPolygon.edges.Count;i++)
			{
				float dist = distanceBwPtAndLine(pt,currShadowPolygon.edges[i]);
				if(dist<0.0f)
					continue;
				if(dist<minDist)
				{
					minDist = dist;
				}
			}
			//
			if(!bNewAlgo)
			{
				//Also get away from all the vertices
				for(int i=0;i<listAllVertices.Count;i++)
				{
					float dist = Vector3.Distance(pt,listAllVertices[0]);

					if(dist<minDist)
					{
						minDist = dist;
					}
				}
			}
			//


			if(minDist>overallMaxDist)
			{
				overallMaxDist = minDist;
				overallMaxDistIndex = probablePointsMinDist.Count;
			}
			probablePointsMinDist.Add(minDist);
		}
		return probablePointsInShadow[overallMaxDistIndex];*/
	}





	private Vector3 findNearestVertex(Vector3 pt,List<Line> allShadowLines)
	{
		List<Vector3> listAllVertices = new List<Vector3> ();
		for(int i=0;i<allShadowLines.Count;i++)
		{
			if(!listAllVertices.Contains(allShadowLines[i].vertex[0]))
			{
				listAllVertices.Add(allShadowLines[i].vertex[0]);
			}
			if(!listAllVertices.Contains(allShadowLines[i].vertex[1]))
			{
				listAllVertices.Add(allShadowLines[i].vertex[1]);
			}
		}
		float minDist = Vector3.Distance (pt, listAllVertices [0]);
		Vector3 selVertex = listAllVertices [0];
		for(int i=1;i<listAllVertices.Count;i++)
		{
			float dist = Vector3.Distance (pt, listAllVertices [i]);
			if(dist<minDist)
			{
				minDist = dist;
				selVertex = listAllVertices [i];
			}
		}
		return selVertex;
	}




	int skipAngleGreedyGlobal = 10;
	int skipAngleNearMissGlobal = 1;
	int skipAngleShadowAssistedGlobal = 45;

	//UPDATE REQUIRED
	//Nearest safepoint. Least movement.
	private Vector3 findNextPosEnemyGreedy(GameObject enemyObj,Vector3 currPosEnemy)
	{
		/*if(pointInShadow(enemyObj.transform.position,nextPlayerPath))
		{
			Debug.Log("Enemy is inside shadow. "+enemyObj.transform.position);
		}
		else
		{
			Debug.Log("Enemy is outside shadow. "+enemyObj.transform.position);
		}*/
		if(!pointInShadow(currPosEnemy,nextPlayerPath))
		{
			float timePlayer = Vector3.Distance(pathPoints[nextPlayerPath],pathPoints[nextPlayerPath-1])/speedPlayer;
			float radiusMovement = speedEnemy*timePlayer;
			float radiiVar = radiusMovement/10;
			float radiiStep = radiiVar;
			while(radiiVar<radiusMovement)
			{
				Vector3 vecSel = currPosEnemy;
				//vecSel.x+=radiiVar;
				int angleVar=0;
				while(!pointInShadow(vecSel,nextPlayerPath) || !CheckStraightLineVisibility(vecSel,currPosEnemy))
				{

					vecSel.x = currPosEnemy.x + radiiVar*Mathf.Cos(angleVar* Mathf.Deg2Rad);
					vecSel.z = currPosEnemy.z + radiiVar*Mathf.Sin(angleVar* Mathf.Deg2Rad);
					angleVar+=skipAngleGreedyGlobal;
					if(angleVar==360)
						break;

				}
				if(pointInShadow(vecSel,nextPlayerPath) && CheckStraightLineVisibility(vecSel,currPosEnemy))
				{
					//Debug.Log("findNextPosEnemyGreedy. NextPos selected to move to is = "+vecSel);
					return vecSel;
				}
				radiiVar+=radiiStep;
			}
		}
		//Debug.Log("findNextPosEnemyGreedy. NextPos selected to move to is = "+enemyObj.transform.position);
		return currPosEnemy;

	}
	//Reaching safest possible point:based on largest angle made in shadow region from start point.select the middle angle
	private Vector3 findNextPosEnemyNearMiss2(GameObject enemyObj,Vector3 currPosEnemy)
	{
		if(!pointInShadow(currPosEnemy,nextPlayerPath))
		{
			float timePlayer = Vector3.Distance(pathPoints[nextPlayerPath],pathPoints[nextPlayerPath-1])/speedPlayer;
			float radiusMovement = speedEnemy*timePlayer;
			int skipAngle=skipAngleNearMissGlobal;
			Vector3 vecSel = currPosEnemy;
			//vecSel.x+=radiusMovement;
			int angleVar=0;
			List<int> listAngles = new List<int>();
			bool insideShadow=false;
			while(true)
			{
				vecSel.x = currPosEnemy.x + radiusMovement*Mathf.Cos(angleVar* Mathf.Deg2Rad);
				vecSel.z = currPosEnemy.z + radiusMovement*Mathf.Sin(angleVar* Mathf.Deg2Rad);
				if(pointInShadow(vecSel,nextPlayerPath) && CheckStraightLineVisibility(vecSel,currPosEnemy))
				{
					if(!insideShadow)
						listAngles.Add(angleVar);
					insideShadow=true;
				}
				else 
				{
					if(insideShadow)
						listAngles.Add(angleVar-skipAngle);
					insideShadow=false;
				}
				angleVar+=skipAngle;
				if(angleVar>=360)
				{
					if(insideShadow)
						listAngles.Add(angleVar-skipAngle);
					break;
				}
				
			}
			int allAnglesCount = listAngles.Count;

			//////////////////////////////////////
			/*string strAngles="( ";
			foreach(int angleTemp in listAngles)
			{
				strAngles+=" , "+angleTemp;
				
			}
			Debug.Log(strAngles+" )");
			*/
			//////////////////////////////////////;

			if(allAnglesCount==0)
				return currPosEnemy;
			int lastProbableInsideAngle = -1;
			if(360%skipAngle!=0)
				lastProbableInsideAngle = 360-(360%skipAngle);
			else
				lastProbableInsideAngle = 360-skipAngle;
			if(listAngles[0]==0 && listAngles[allAnglesCount-1]==lastProbableInsideAngle)
			{
				listAngles[allAnglesCount-1]=listAngles[1]+360;
				listAngles.RemoveAt(0);
				listAngles.RemoveAt(0);
				allAnglesCount = listAngles.Count;
			}
			int maxDiff=-1;
			int indxMaxDiff=-1;

			for(int j=0;j<=allAnglesCount-2;j+=2)
			{
				if(listAngles[j+1]-listAngles[j]>maxDiff)
				{
					indxMaxDiff=j;
					maxDiff = listAngles[j+1]-listAngles[j];
				}
			}
			//////////////////////////////////////
			/*strAngles="( ";
			foreach(int angleTemp in listAngles)
			{
				strAngles+=" , "+angleTemp;
				
			}
			Debug.Log(strAngles+" ))) indxMaxDiff = "+indxMaxDiff);
			*/
			//////////////////////////////////////;
			int angleSel = -1;
			if(indxMaxDiff==-1)
				angleSel = listAngles[0];
			else
			{
				angleSel = (listAngles[indxMaxDiff+1]+listAngles[indxMaxDiff])/2;
				//angleSel = getAngleWithMaxPotential(listAngles[indxMaxDiff],listAngles[indxMaxDiff+1],enemyObj.transform.position,radiusMovement);
			}
			vecSel.x = currPosEnemy.x + radiusMovement*Mathf.Cos(angleSel* Mathf.Deg2Rad);
			vecSel.z = currPosEnemy.z + radiusMovement*Mathf.Sin(angleSel* Mathf.Deg2Rad);
			return vecSel;
		}
		return currPosEnemy;
	}
	private int getAngleWithMaxPotential(int angle1,int angle2,Vector3 currPos,float radiusMovement)
	{
		int skipAngle = 1;
		int tempAngle = angle1;
		int maxCounterShadowTemp = -1;
		int bestAngle = -1;
		while(tempAngle<=angle2)
		{
			Vector3 newCentre = new Vector3();
			newCentre.x = currPos.x + radiusMovement*Mathf.Cos(tempAngle* Mathf.Deg2Rad);
			newCentre.y = 1;
			newCentre.z = currPos.z + radiusMovement*Mathf.Cos(tempAngle* Mathf.Deg2Rad);
			Vector3 tempPoint = new Vector3();
			tempPoint.y = 1;
			int counterShadowTemp = 0;
			for(int theta=0;theta<360;theta+=skipAngle)
			{
				tempPoint.x = newCentre.x + radiusMovement*Mathf.Cos(theta* Mathf.Deg2Rad);
				tempPoint.z = newCentre.z + radiusMovement*Mathf.Cos(theta* Mathf.Deg2Rad);

				if(pointInShadow(tempPoint,nextPlayerPath) || CheckIfInsidePolygon(tempPoint))
				{
					counterShadowTemp++;
				}
			}
			if(maxCounterShadowTemp<counterShadowTemp)
			{
				maxCounterShadowTemp = counterShadowTemp;
				bestAngle = tempAngle;
			}
			tempAngle+=skipAngle;
		}
		return bestAngle;
	}
	/*private Vector3 findNextPosEnemyNearMiss3(GameObject enemyObj)
	{
		if(!pointInShadow(enemyObj.transform.position,nextPlayerPath))
		{
			float timePlayer = Vector3.Distance(pathPoints[nextPlayerPath],pathPoints[nextPlayerPath-1])/speedPlayer;
			float radiusMovement = speedEnemy*timePlayer;
			Vector3 vecSel = enemyObj.transform.position;
			//vecSel.x+=radiusMovement;
			int angleVar=0;
			bool insideShadow=false;
			//check all lines and find nearest visibility edge
			Geometry visiblePolyTemp = (Geometry)hVisiblePolyTable [pathPoints [nextPlayerPath]];
			float minDist = 0f;
			float distTemp = 0f;
			Line firstEdge = visiblePolyTemp.edges[0];
			float y2 = firstEdge.vertex[1].z;
			float y1 = firstEdge.vertex[0].z;
			float x2 = firstEdge.vertex[1].x;
			float x1 = firstEdge.vertex[0].x;
			minDist = Mathf.Abs((y2-y1)*vecSel.x - (x2-x1)*vecSel.z + x2*y1 - y2*x1);
			minDist = minDist/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
			int selEdgeIndx = 0;
			for(int i=1;i<visiblePolyTemp.edges.Count;i++)
			{
				firstEdge = visiblePolyTemp.edges[i];
				y2 = firstEdge.vertex[1].z;
				y1 = firstEdge.vertex[0].z;
				x2 = firstEdge.vertex[1].x;
				x1 = firstEdge.vertex[0].x;
				distTemp = Mathf.Abs((y2-y1)*vecSel.x - (x2-x1)*vecSel.z + x2*y1 - y2*x1);
				distTemp = distTemp/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
				if(distTemp<minDist)
				{
					selEdgeIndx = i;
					minDist = distTemp;
				}
			}
			vecSel.x = enemyObj.transform.position.x + radiusMovement*Mathf.Cos(angleSel* Mathf.Deg2Rad);
			vecSel.z = enemyObj.transform.position.z + radiusMovement*Mathf.Sin(angleSel* Mathf.Deg2Rad);
			return vecSel;
		}
		return enemyObj.transform.position;

	}
	*/
	private void setUpEnemyInitialPos()
	{
		if(m_EnemyStatic)
		{
			createDiscreteMap ();
			sbyte[,] shadowArray = null;
			shadowArray = findSafestSpots();
			placeEnemyStatic(shadowArray);
		}
		if(m_Greedy)
		{
			setGlobalVars1();
			int numGreedy = m_nEnemyGreedy;
			while(numGreedy>0)
			{
				Vector3 sel = selectInitialGreedyRandomPos();
				placeEnemyGreedyAt(sel);
				numGreedy--;
			}
		}
		if(m_NearMiss)
		{
			setGlobalVars1();
			int numNearMiss = m_nEnemyNearMiss;
			while(numNearMiss>0)
			{
				Vector3 sel = selectInitialNearMissRandomPos();
				//sel.x=5.3f;
				//sel.y=1.0f;
				//sel.z=2.2f;
				placeEnemyNearMissAt(sel);
				numNearMiss--;
			}
		}
		if(m_ShadowEdgeAssisted)
		{
			setGlobalVars1();

			Vector3 sel = selectInitialShadowAssistedRandomPos();
			placeEnemyShadowAssistedAt(sel);


		}
		if(m_nEnemyCentroid>0)
		{
			setGlobalVars1();
			int numCentroid = m_nEnemyCentroid;
			while(numCentroid>0)
			{
				Vector3 sel = selectInitialNearMissRandomPos();
				placeEnemyCentroidAt(sel);
				numCentroid--;
			}
		}
	}

	//TODO :Error while placing enemies. Maybe random placement works.
	private void placeEnemyCentroid()
	{
		/*int numCentroidEnemies = m_nEnemyCentroid;
		while(numCentroidEnemies>0)
		{
			GameObject enemyObj = Instantiate(enemyPrefab) as GameObject;
			Component.Destroy (enemyObj.GetComponent("Enemy"));
			List<Vector3> centrePts = (List<Vector3>)hCentroidShadows[pathPoints[0]];
			enemyObj.transform.position = centrePts[Random.Range(0,centrePts.Count-1)];//[m_nEnemyCentroid-numCentroidEnemies];
			//m_enemyNextPosCentroidList.Add(((List<Vector3>)hCentroidShadows[pathPoints[1]])[m_nEnemyCentroid-numCentroidEnemies]);

			m_enemyCentroidList.Add(enemyObj);
			m_enemyNextPosCentroidList.Add(findNextPosEnemyCentroid(enemyObj));
			numCentroidEnemies--;
		}*/
	}
	//TODO:Incomplete
	private Vector3 findNextPosEnemyCentroid(GameObject enemyObj)
	{
		Vector3 vecSel = enemyObj.transform.position;
		List<Geometry> shadowPolygonsTemp = (List<Geometry>)hTable [pathPoints [nextPlayerPath]];

		List<Line> allShadowEdges = new List<Line> ();
		foreach(Geometry shadowGeo in shadowPolygonsTemp)
		{
			allShadowEdges.AddRange(shadowGeo.edges);
		}
		float minDist = 0.0f;
		float distTemp = 0.0f;
		Line firstEdge = allShadowEdges[0];
		float y2 = firstEdge.vertex[1].z;
		float y1 = firstEdge.vertex[0].z;
		float x2 = firstEdge.vertex[1].x;
		float x1 = firstEdge.vertex[0].x;
		minDist = Mathf.Abs((y2-y1)*vecSel.x - (x2-x1)*vecSel.z + x2*y1 - y2*x1);
		minDist = minDist/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
		int selEdgeIndx = 0;
		for(int i=1;i<allShadowEdges.Count;i++)
		{
			firstEdge = allShadowEdges[i];
			y2 = firstEdge.vertex[1].z;
			y1 = firstEdge.vertex[0].z;
			x2 = firstEdge.vertex[1].x;
			x1 = firstEdge.vertex[0].x;
			distTemp = Mathf.Abs((y2-y1)*vecSel.x - (x2-x1)*vecSel.z + x2*y1 - y2*x1);
			distTemp = distTemp/Mathf.Sqrt(Mathf.Pow((y2-y1),2) + Mathf.Pow((x2-x1),2));
			if(distTemp<minDist)
			{
				selEdgeIndx = i;
				minDist = distTemp;
			}
		}
		
		return vecSel;
	}
	GameObject scaleCharacter(GameObject enemyObj)
	{
		float scalingTemp = 1.0f;
		if(currSceneName=="myCrash.unity" || currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			scalingTemp = 0.57f;
		}
		Vector3 lscale= enemyObj.transform.localScale;
		lscale.x*=playerScaleForCurrent*scalingTemp;
		lscale.y*=playerScaleForCurrent*scalingTemp;
		lscale.z*=playerScaleForCurrent*scalingTemp;
		Renderer rend = enemyObj.GetComponent<Renderer>();
		rend.transform.localScale = lscale;
		return enemyObj;
	}
	private void placeEnemyCentroidAt(Vector3 sel)
	{
		GameObject enemyObj = Instantiate(enemyPrefab) as GameObject;

		Vector3 lscale= enemyObj.transform.localScale;
		lscale.x*=playerScaleForCurrent;
		lscale.y*=playerScaleForCurrent;
		lscale.z*=playerScaleForCurrent;
		Renderer rend = enemyObj.GetComponent<Renderer>();
		rend.transform.localScale = lscale;
	
		Component.Destroy (enemyObj.GetComponent("Enemy"));
		enemyObj.transform.position = sel;
		EnemyMovement centroidObj = new EnemyMovement();
		centroidObj.bCaught = false;
		centroidObj.enemyObj = enemyObj;
		centroidObj.vNextPos.Add (findNextPosEnemyCentroid(enemyObj));
		m_enemyCentroidList.Add(centroidObj);
		
	}
	private void placeEnemyGreedyAt(Vector3 sel)
	{
		GameObject enemyObj = Instantiate(enemyPrefab) as GameObject;

		Vector3 lscale= enemyObj.transform.localScale;
		lscale.x*=playerScaleForCurrent;
		lscale.y*=playerScaleForCurrent;
		lscale.z*=playerScaleForCurrent;
		Renderer rend = enemyObj.GetComponent<Renderer>();
		rend.transform.localScale = lscale;
	
		Component.Destroy (enemyObj.GetComponent("Enemy"));
		enemyObj.transform.position = sel;
		EnemyMovement greedyObj = new EnemyMovement();
		greedyObj.bCaught = false;
		greedyObj.startPosIndx = (Vector2)h_mapPtToIndx [sel];
		//Debug.Log ("greedyObj.startPosIndx = "+greedyObj.startPosIndx);
		greedyObj.enemyObj = enemyObj;
		greedyObj.vNextPos.Add (findNextPosEnemyGreedy(enemyObj,enemyObj.transform.position));
		m_enemyGreedyList.Add(greedyObj);
	}
	
	private void placeEnemyNearMissAt(Vector3 sel)
	{
		GameObject enemyObj = Instantiate(enemyPrefab) as GameObject;

		Vector3 lscale= enemyObj.transform.localScale;
		lscale.x*=playerScaleForCurrent;
		lscale.y*=playerScaleForCurrent;
		lscale.z*=playerScaleForCurrent;
		Renderer rend = enemyObj.GetComponent<Renderer>();
		rend.transform.localScale = lscale;
	
		Component.Destroy (enemyObj.GetComponent("Enemy"));
		enemyObj.transform.position = sel;
		EnemyMovement nearMissObj = new EnemyMovement();
		nearMissObj.bCaught = false;
		nearMissObj.startPosIndx = (Vector2)h_mapPtToIndx [sel];
		nearMissObj.enemyObj = enemyObj;
		nearMissObj.vNextPos.Add (findNextPosEnemyNearMiss2(enemyObj,enemyObj.transform.position));
		m_enemyNearMissList.Add(nearMissObj);
			
	}
	private void placeEnemyShadowAssistedAt(Vector3 sel)
	{
		GameObject enemyObj = Instantiate(enemyPrefab) as GameObject;

		//navjot
		//enemyObj.GetComponent<Renderer>().enabled = false;

		Vector3 lscale= enemyObj.transform.localScale;
		lscale.x*=playerScaleForCurrent;
		lscale.y*=playerScaleForCurrent;
		lscale.z*=playerScaleForCurrent;
		Renderer rend = enemyObj.GetComponent<Renderer>();
		rend.transform.localScale = lscale;
	
		Component.Destroy (enemyObj.GetComponent("Enemy"));
		enemyObj.transform.position = sel;
		EnemyMovement shadowAssistedObj = new EnemyMovement();
		shadowAssistedObj.bCaught = false;
		shadowAssistedObj.startPosIndx = (Vector2)h_mapPtToIndx [sel];
		shadowAssistedObj.enemyObj = enemyObj;
		shadowAssistedObj.vNextPos.Add (findNextPosEnemyShadowAssisted(enemyObj,enemyObj.transform.position));
		m_enemyShadowAssistedList.Add(shadowAssistedObj);
	}

	private Vector3 selectInitialNearMissRandomPos()
	{
		//sbyte[,] shadowArray = (sbyte[,])h_discreteShadows [pathPoints [0]];
		while(true)
		{
			int selX = Random.Range(0,discretePtsX);
			int selZ = Random.Range(0,discretePtsZ);
			Vector3 sel = (Vector3)h_mapIndxToPt[new Vector2(selX,selZ)];
			while(!pointInShadow(sel,0))
			{
				selX = Random.Range(0,discretePtsX);
				selZ = Random.Range(0,discretePtsZ);
				sel = (Vector3)h_mapIndxToPt[new Vector2(selX,selZ)];
			}

			bool selAgain = false;
			foreach(EnemyMovement nearMissObjTemp in m_enemyNearMissList)
			{
				if(Vector3.Distance(nearMissObjTemp.enemyObj.transform.position,sel)<1.0f)
				{
					selAgain=true;
					break;
				}
			}
			if(selAgain)
			{
				continue;
			}
			return sel;
		}
	}
	private Vector3 selectInitialGreedyRandomPos()
	{
		int selX = Random.Range(0,discretePtsX);
		int selZ = Random.Range(0,discretePtsZ);
		Vector3 sel = (Vector3)h_mapIndxToPt[new Vector2(selX,selZ)];
		while(!pointInShadow(sel,0))
		{
			selX = Random.Range(0,discretePtsX);
			selZ = Random.Range(0,discretePtsZ);
			sel = (Vector3)h_mapIndxToPt[new Vector2(selX,selZ)];
		}
		
		return sel;
		
	}
	private Vector3 selectInitialShadowAssistedRandomPos()
	{
	
	
		int selX = Random.Range(0,discretePtsX);
		int selZ = Random.Range(0,discretePtsZ);
		Vector3 sel = (Vector3)h_mapIndxToPt[new Vector2(selX,selZ)];
		while(!pointInShadow(sel,0))
		{
			selX = Random.Range(0,discretePtsX);
			selZ = Random.Range(0,discretePtsZ);
			sel = (Vector3)h_mapIndxToPt[new Vector2(selX,selZ)];
		}

		return sel;

	}
	private void placeEnemyStatic(sbyte[,] shadowArray)
	{
		List<Vector3> listHiddenSpots = new List<Vector3> ();
		for(int j=0;j<discretePtsX;j++)
		{
			for(int k=0;k<discretePtsZ;k++)
			{
				if(shadowArray[j,k]==0)
				{
					listHiddenSpots.Add((Vector3)h_mapIndxToPt[new Vector2(j,k)]);
				}
			}
		}
		int numStatic = m_nEnemyStatic;
		while(numStatic>0 && listHiddenSpots.Count>0)
		{
			int sel = Random.Range (0, listHiddenSpots.Count);
			Vector3 selVec = listHiddenSpots[sel];
			GameObject enemyObj = Instantiate(enemyPrefab) as GameObject;
			Component.Destroy (enemyObj.GetComponent("Enemy"));
			enemyObj.transform.position = selVec;
			listHiddenSpots.RemoveAt(sel);
			numStatic--;
		}
	}
	//Unused
	/*
	private Vector3 findCentroid1(Geometry geo)
	{

		float radius_hiddenSphere = ((SphereCollider)hiddenSphere.collider).radius*((SphereCollider)hiddenSphere.collider).transform.lossyScale.x;
		int numPts = 0;
		float meanX = 0f;
		float meanZ = 0f;
		for(float j=m_minX;j<m_maxX;j+=m_step)
		{
			for(float k=m_minZ;k<m_maxZ;k+=m_step)
			{
				Vector3 pt = new Vector3(j,1,k);

				if(geo.PointInside(pt) && !Physics.CheckSphere(pt,radius_hiddenSphere))
				{
					numPts++;
					meanX+=j;
					meanZ+=k;
				}

			}
		}
		meanX = meanX / numPts;
		meanZ = meanZ / numPts;
		return new Vector3 (meanX, 1, meanZ);
	}
	*/
	private void setGlobalVars1()
	{
		m_minX = mapBoundary[0].x;
		m_minZ = mapBoundary[0].z;
		m_maxX = mapBoundary[0].x;
		m_maxZ = mapBoundary[0].z;
		for(int i=1;i<4;i++)
		{
			if(m_minX>mapBoundary[i].x)
			{
				m_minX=mapBoundary[i].x;
			}
			if(m_minZ>mapBoundary[i].z)
			{
				m_minZ=mapBoundary[i].z;
			}
			if(m_maxX<mapBoundary[i].x)
			{
				m_maxX=mapBoundary[i].x;
			}
			if(m_maxZ<mapBoundary[i].z)
			{
				m_maxZ=mapBoundary[i].z;
			}
		}
		int Indx = 0;
		
		
		discretePtsX = (int)(((m_maxX - m_minX) / m_step)+0.5);
		discretePtsZ = (int)(((m_maxZ - m_minZ) / m_step)+0.5);
		Debug.Log("discretePtsX = "+discretePtsX);
		Debug.Log("discretePtsZ = "+discretePtsZ);
		
		int j1=0;
		for(float j=m_minX;j<m_maxX && j1<discretePtsX;j+=m_step)
		{
			int k1=0;
			for(float k=m_minZ;k<m_maxZ && k1<discretePtsZ;k+=m_step)
			{
				Vector3 pt = new Vector3(j,1,k);
				Vector2 keyTemp = new Vector2((float)j1,(float)k1);
				if(!h_mapIndxToPt.ContainsKey(keyTemp))
				{
					h_mapIndxToPt.Add(keyTemp,pt);
				}
				if(!h_mapPtToIndx.ContainsKey(pt))
				{
					h_mapPtToIndx.Add(pt,keyTemp);
					
				}
				k1++;
			}
			j1++;
		}
		
	}
	private void setGlobalVars1_Old()
	{
		m_minX = mapBoundary[0].x;
		m_minZ = mapBoundary[0].z;
		m_maxX = mapBoundary[0].x;
		m_maxZ = mapBoundary[0].z;
		for(int i=1;i<4;i++)
		{
			if(m_minX>mapBoundary[i].x)
			{
				m_minX=mapBoundary[i].x;
			}
			if(m_minZ>mapBoundary[i].z)
			{
				m_minZ=mapBoundary[i].z;
			}
			if(m_maxX<mapBoundary[i].x)
			{
				m_maxX=mapBoundary[i].x;
			}
			if(m_maxZ<mapBoundary[i].z)
			{
				m_maxZ=mapBoundary[i].z;
			}
		}
		int Indx = 0;

		
		discretePtsX = (int)(((m_maxX - m_minX) / m_step)+0.5);
		discretePtsZ = (int)(((m_maxZ - m_minZ) / m_step)+0.5);
		Debug.Log("discretePtsX = "+discretePtsX);
		Debug.Log("discretePtsZ = "+discretePtsZ);

		int j1=0;
		for(float j=m_minX;j<m_maxX && j1<discretePtsX;j+=m_step)
		{
			int k1=0;
			for(float k=m_minZ;k<m_maxZ && k1<discretePtsZ;k+=m_step)
			{
				Vector3 pt = new Vector3(j,1,k);
				Vector2 keyTemp = new Vector2((float)j1,(float)k1);
				if(!h_mapIndxToPt.ContainsKey(keyTemp))
				{
					h_mapIndxToPt.Add(keyTemp,pt);
				}
				if(!h_mapPtToIndx.ContainsKey(pt))
				{
					h_mapPtToIndx.Add(pt,keyTemp);

				}
				k1++;
			}
			j1++;
		}

	}
	/*
	private void findEnemyPath2 ()
	{
		setGlobalVars1 ();
		foreach(Vector3 pathPt in pathPoints)
		{
			List<Geometry> shadows = (List<Geometry>)hTable[pathPt];
			List<Vector3> centroidPtsList = new List<Vector3>();
 			foreach(Geometry geo in shadows)
			{
				Vector3 centroidPt = findCentroid1(geo);
				centroidPtsList.Add(centroidPt);
			}
			hCentroidShadows.Add(pathPt,centroidPtsList);
		}
	}

	private List<Vector3> findEnemyPath1 ()
	{
		enemyPath = new List<Vector3>();

		int centralityIndx = -1;
		int row = -1;
		int col = -1;
		//List<Vector2> centreList = new List<Vector2>();
		int numSpots = 1;
		//float minDist = 5;
		for(int i=0;i<pathPoints.Count;i++)
		{
			sbyte[,] shadowArray = (sbyte[,])h_discreteShadows [pathPoints [i]];
			for(int j=0;j<discretePtsX;j++)
			{
				for(int k=0;k<discretePtsZ;k++)
				{
					if(shadowArray[j,k]==0)
					{
						int centralityIndxTemp = centralityCalc(shadowArray,j,k);
						if(centralityIndx<=centralityIndxTemp)
						{
							centralityIndx = centralityIndxTemp;
							row = j;
							col = k;

						}
					}
				}
			}
			enemyPath.Add((Vector3)h_mapIndxToPt[new Vector2(row,col)]);
			//centreList.RemoveAt(0);
		}
		return enemyPath;
	}
	*/
	/*
	private int centralityCalc(sbyte[,] shadowArray,int j,int k)
	{
		int rowJ = j;
		int colK = k;
		int centralityIndx = 0;
		int antiCentralityIndx = 0;
		while(true)
		{
			rowJ--;
			colK--;
			int rowLen = (j - rowJ)*2 +1;
			if(rowJ<0 || colK<0 || rowJ+rowLen>discretePtsX || colK+rowLen>discretePtsZ)
				break;
			int centralityIndxTemp = 0;
			int antiCentralityIndxTemp = 0;
			for(int i1=rowJ;i1<rowLen;i1++)
			{
				if(shadowArray[i1,colK]==0)
				{
					centralityIndxTemp++;
				}
				else
				{
					antiCentralityIndxTemp++;
				}
				if(shadowArray[i1,colK+rowLen-1]==0)
				{
					centralityIndxTemp++;
				}
				else
				{
					antiCentralityIndxTemp++;
				}
			}
			for(int i2=colK+1;i2<rowLen-1;i2++)
			{
				if(shadowArray[rowJ,i2]==0)
				{
					centralityIndxTemp++;
				}
				else
				{
					antiCentralityIndxTemp++;
				}
				if(shadowArray[rowJ+rowLen-1,i2]==0)
				{
					centralityIndxTemp++;
				}
				else
				{
					antiCentralityIndxTemp++;
				}
			}
			if(antiCentralityIndxTemp>centralityIndxTemp/4)
			//if(antiCentralityIndx>centralityIndx/10)
				break;
			centralityIndx+=centralityIndxTemp;
			antiCentralityIndx+=antiCentralityIndxTemp;
		}
		return centralityIndx- antiCentralityIndx;
	}
	private void placeEnemy(sbyte[,] shadowArray,GameObject enemyObj)
	{
		//Algo: find centre and place there
		int centralityIndx = -1;
		int row = -1;
		int col = -1;
		List<Vector2> centreList = new List<Vector2>();
		int numSpots = 1;
		//float minDist = 5;
		for(int j=0;j<discretePtsX;j++)
		{
			for(int k=0;k<discretePtsZ;k++)
			{
				if(shadowArray[j,k]==0)
				{
					int centralityIndxTemp = centralityCalc(shadowArray,j,k);
					if(centralityIndx<=centralityIndxTemp)
					{
						centralityIndx = centralityIndxTemp;
						row = j;
						col = k;
						centreList.Insert(0,new Vector2(row,col));
						if(centreList.Count>numSpots)
						{
							centreList.RemoveAt(numSpots);
						}
					}
				}
			}
		}
		enemyObj.transform.position = (Vector3)h_mapIndxToPt[new Vector2(row,col)];
		centreList.RemoveAt (0);
		foreach(Vector2 vect in centreList)
		{
			GameObject enemyObj1 = Instantiate(enemyPrefab) as GameObject;
			Component.Destroy (enemyObj1.GetComponent("Enemy"));
			enemyObj1.transform.position = (Vector3)h_mapIndxToPt[vect];
		}
	}*/
	private sbyte[,] findSafestSpots()
	{
		Debug.Log("h_mapIndxToPt.Count = "+h_mapIndxToPt.Keys.Count);
		Debug.Log ("h_discreteShadows.Count = "+h_discreteShadows.Keys.Count);
		sbyte[,] shadowArrayFirst = (sbyte[,])h_discreteShadows [pathPoints [0]];
		sbyte[,] shadowArray = new sbyte[discretePtsX,discretePtsZ];
		sbyte[,] shadowArrayTemp = new sbyte[discretePtsX,discretePtsZ];
		int pathPointsCount = pathPoints.Count;
		//float from_factor = 0.0f;
		//float to_factor = 1.0f;
		while(true)
		{
			System.Array.Copy(shadowArrayFirst,shadowArray,discretePtsX*discretePtsZ);
			for(int i=1;i<pathPointsCount;i++)
			{
				System.Array.Copy(shadowArray,shadowArrayTemp,discretePtsX*discretePtsZ);
				shadowArray = findCommonInArray2D(shadowArray,(sbyte[,])h_discreteShadows [pathPoints [i]]);
				if(checkForNullShadow(shadowArray))
				{
					return shadowArrayTemp;
				}
			}
			break;
			/*if(checkForNullShadow(shadowArray))
			{
				shadowArray = new sbyte[discretePtsX,discretePtsZ];
				to_factor = factor;
				//from_factor/=2;
				pathPointsCount=pathPointsCount*factor;
			}
			else
			{
				float old_factorTemp = old_factor;
				old_factor = factor;
				factor = (old_factorTemp+factor)/2;
				pathPointsCount=pathPointsCount*factor;
				break;
			}
			*/
		}
		//visualizeSafeSpots (shadowArray);
		return shadowArray;
	}
	private bool checkForNullShadow(sbyte[,] array1)
	{
		int shadowPointCounter = 0;
		for(int j=0;j<discretePtsX;j++)
		{
			for(int k=0;k<discretePtsZ;k++)
			{
				if(array1[j,k]==0)
				{
					shadowPointCounter++;
				}
			}
		}
		if (shadowPointCounter > 0)
		{
			return false;
		}
		else
		{
			return true;
		}
	}
	private sbyte[,] findCommonInArray2D(sbyte[,] array1,sbyte[,] array2)
	{
		sbyte[,] shadowArray = new sbyte[discretePtsX,discretePtsZ];
		for(int j=0;j<discretePtsX;j++)
		{
			for(int k=0;k<discretePtsZ;k++)
			{
				shadowArray[j,k] = (array1[j,k] > array2[j,k])?array1[j,k]:array2[j,k];
			}
		}
		return shadowArray;
	}
	private void visualizeSafeSpots(sbyte[,] visibleArray2D)
	{
		for(int j=0;j<discretePtsX;j++)
		{
			for(int k=0;k<discretePtsZ;k++)
			{
				if(visibleArray2D[j,k]==0)
				{
					GameObject clone1 = (GameObject)Instantiate(hiddenSphere);
					Debug.Log("visualizeSafeSpots for pt = "+j+" , "+k);
					clone1.transform.position = (Vector3)h_mapIndxToPt[new Vector2(j,k)];
					//Debug.Log("clone1.transform.position = "+clone1.transform.position);
					//hiddenSphereList.Add(clone1);
				}
			}
		}
	}

	private void makeBox() 
	{
		Debug.Log("In makeBox");
		//Ensures the bottom left and top right values are correct
		//regardless of how the user boxes units
		float xmin = Mathf.Min(start_box.x, end_box.x);
		float zmin = Mathf.Min(start_box.z, end_box.z);
		float width = Mathf.Max(start_box.x, end_box.x) - xmin;
		float height = Mathf.Max(start_box.z, end_box.z) - zmin;
		boundbox = new Rect(xmin, zmin, width, height);
		if(width*height>0.01)
		{
			selectedBox = Instantiate(selectedBoxPrefab) as GameObject;
			b_ShowBoundbox = true;
			float centreX=(start_box.x+end_box.x)/2;
			float centreZ=(start_box.z+end_box.z)/2;
			selectedBox.GetComponent<Renderer>().enabled=true;
			Vector3 tempVect = new Vector3(centreX,1,centreZ);
			selectedBox.transform.position=tempVect;
			tempVect.x=width;
			tempVect.z=height;
			selectedBox.transform.localScale=tempVect;

			//////////////////////////////////////////
			/// //Identify path points in box
			Geometry boundboxGeo = new Geometry ();
			boundboxGeo.edges.Add (new Line (new Vector3(boundbox.x,1,boundbox.y),new Vector3(boundbox.x+boundbox.width,1,boundbox.y)));
			boundboxGeo.edges.Add (new Line (new Vector3(boundbox.x+boundbox.width,1,boundbox.y),new Vector3(boundbox.x+boundbox.width,1,boundbox.y+boundbox.height)));
			boundboxGeo.edges.Add (new Line (new Vector3(boundbox.x+boundbox.width,1,boundbox.y+boundbox.height),new Vector3(boundbox.x,1,boundbox.y+boundbox.height)));
			boundboxGeo.edges.Add (new Line (new Vector3(boundbox.x,1,boundbox.y+boundbox.height),new Vector3(boundbox.x,1,boundbox.y)));
			startIndex = -1;
			endIndex = -1;
			int currIndex = 0;
			foreach(Vector3 vect in pathPoints)
			{
				if(boundboxGeo.PointInside(vect))
				{
					if(startIndex==-1)
					{
						startIndex=currIndex;
					}
				}
				else
				{
					if(startIndex!=-1)
					{
						endIndex=currIndex-1;
						break;
					}
				}
				currIndex++;
			}

			//if all points selected
			if(startIndex!=-1 && endIndex==-1)
			{
				endIndex=currIndex-1;
			}

			Debug.Log ("startIndex = " + startIndex);
			Debug.Log ("endIndex = " + endIndex);
			/// /////////////////////////////////////////
			//Only show shadow polygon if only one path point is selected

		}
		else
		{
			GameObject.Destroy(selectedBox);
			b_ShowBoundbox=false;
		}
		if(hiddenSphereList!=null)
		{
			foreach(GameObject g in hiddenSphereList)
			{
				GameObject.Destroy(g);
			}
		}
		hiddenSphereList=null;
		Debug.Log ("Destroying shadowMeshes = " + shadowMeshes.Count);
		foreach(GameObject tempObj in shadowMeshes)
		{
			GameObject.Destroy(tempObj);
		}
		shadowMeshes.Clear ();
	}
	public bool checkIfLineExists(Line lineTemp,List<Line> listEdges)
	{
		foreach(Line l in listEdges)
		{
			if(l.Equals(lineTemp))
				return true;
		}
		return false;
	}
	/// <summary>
	/// for use in consolidate polygons
	////// </summary>
	/// <returns><c>true</c>, if edges was compared, <c>false</c> otherwise.</returns>
	/// <param name="l1">L1.</param>
	/// <param name="l2">L2.</param>
	/*private bool compareEdges(Line l1,Line l2)
	{
		float limit = 0.1f;
		float minVal = Vector3.Distance(l1.MidPoint(),l2.MidPoint());
		if (minVal <= limit)
		{
			if((comparePoints(l1.vertex[0],l2.vertex[1]) && comparePoints(l1.vertex[1],l2.vertex[0])) || (comparePoints(l1.vertex[0],l2.vertex[0]) && comparePoints(l1.vertex[1],l2.vertex[1])))
				return true;
		}
		return false;
	}
	private bool comparePoints(Vector3 v1,Vector3 v2)
	{
		float limit = 0.1f;
		float minVal = Vector3.SqrMagnitude (v1 - v2);
		if (minVal <= limit)
			return true;
		return false;
	}*/
	/*
	public Geometry consolidateShadowPolygon(Geometry geo)
	{
		//Removing duplicate points
		StandardPolygon sdTemp = new StandardPolygon();
		bool addAnother = false;
		Geometry newGeo = new Geometry ();
		foreach(Line l1 in geo.edges)
		{
			Line l = l1;
			addAnother = sdTemp.addPoint(l.vertex[0]);
			if(!addAnother)
			{
				int indx = sdTemp.findIndexOfDuplicate(l.vertex[0]);
				Vector3 dupPt = sdTemp.getVertices()[indx];
				l.vertex[0]=dupPt;
			}
			addAnother = sdTemp.addPoint(l.vertex[1]);
			if(!addAnother)
			{
				int indx = sdTemp.findIndexOfDuplicate(l.vertex[1]);
				Vector3 dupPt = sdTemp.getVertices()[indx];
				l.vertex[1]=dupPt;
			}
			newGeo.edges.Add(l);
		}
		//Removing very close edges
		for(int i=0;i<newGeo.edges.Count;i++)
		{
			for(int j=i+1;j<newGeo.edges.Count;j++)
			{
				//if(compareEdges(newGeo.edges[i],newGeo.edges[j]))
				if(newGeo.edges[i].Equals(newGeo.edges[j]))
				{
					newGeo.edges.RemoveAt(j);
					j--;
				}
			}
		}
		return newGeo;
		//Removing very small lines
		List<int> tobeRemoved = new List<int> ();
		for(int i=0;i<newGeo.edges.Count;i++)
		{
			if(newGeo.edges[i].LengthOfLine()<0.1)
			{
				Vector3 v1 = newGeo.edges[i].vertex[0];
				Vector3 v2 = newGeo.edges[i].vertex[1];
				Vector3 midPt = newGeo.edges[i].vertex[0];//newGeo.edges[i].MidPoint();
				bool bV1Done = false;
				bool bV2Done = false;
				int j1=-1;
				int j2=-1;
				for(int j=0;i<newGeo.edges.Count;j++)
				{
					if(i==j)
						continue;
					if(!bV1Done && newGeo.edges[j].vertex[0].Equals(v1))
					{
						newGeo.edges[j].vertex[0] = midPt;

						bV1Done=true;
					}
					if(!bV1Done && newGeo.edges[j].vertex[1].Equals(v1))
					{
						newGeo.edges[j].vertex[1] = midPt;

						bV1Done=true;
					}
					if(!bV2Done && newGeo.edges[j].vertex[0].Equals(v2))
					{
						newGeo.edges[j].vertex[0] = midPt;

						bV2Done=true;
					}
					if(!bV2Done && newGeo.edges[j].vertex[1].Equals(v2))
					{
						newGeo.edges[j].vertex[1] = midPt;

						bV2Done=true;
					}
				}
				//newGeo.edges.RemoveAt(i);
				//i--;

				if(bV1Done && bV2Done)
				{
					tobeRemoved.Add(i);
				}
				else
				{
					Debug.Log ("&&&&&&&&&&&& Error while removing small edges &&&&&&&&&&&&");
				}
			}
		}
		foreach(int i in tobeRemoved)
		{
			newGeo.edges.RemoveAt(i);
		}
		return newGeo;

		foreach(Line l in geo.edges)
		{
			sdTemp.addPoint(l.vertex[0]);
			sdTemp.addPoint(l.vertex[1]);
		}
		sdTemp.removeDuplicates();
		//Debug.Log("sdTemp.getVertices().Count = "+sdTemp.getVertices().Count);
		Debug.Log("geo.edges.Count = "+geo.edges.Count);
		List<Vector3> copyPts = new List<Vector3> ();
		copyPts.AddRange (sdTemp.getVertices ());
		Geometry geo2 = new Geometry ();
		for(int i=0;i<copyPts.Count;i++)
		{
			for(int j=0;j<copyPts.Count;j++)
			{
				if(i==j)
					continue;
				Line lineTemp = new Line(copyPts[i],copyPts[j]);
				foreach(Line l in geo.edges)
				{
					if(l.Equals(lineTemp))
					{
						if(!checkIfLineExists(lineTemp,geo2.edges))
							geo2.edges.Add(lineTemp);
					}
				}
			}
		}
		//geo2.edges.AddRange (hiddenLines);
		return geo2;
	}
*/
	private List<Line> removeLineFromList(List<Line> allEdges1,Line edgeToRemove)
	{
		int counter = 0;
		foreach(Line l in allEdges1)
		{
			if(l.Equals(edgeToRemove))
			{
				allEdges1.RemoveAt(counter);
				//Debug.Log("Removed edge");
				break;
			}
			counter++;
		}
		List<Line> allEdges = new List<Line> ();
		allEdges.AddRange (allEdges1);
		return allEdges;
	}
	private List<Vector3> arrangePointsFromPoint(Vector3 pPoint, List<Vector3> endPts)
	{
		Vector3 normalVect = new Vector3 (0, 1, 0);
		Vector3 xVect = new Vector3 (1, 0, 0);
		Vector3 alongX = new Vector3(pPoint.x+2,pPoint.y,pPoint.z);

		List<Vector3> arrangedPoints = new List<Vector3> ();
		List<float> angles = new List<float>();
		
		foreach(Vector3 vect in endPts)
		{
			float sAngle = SignedAngleBetween(pPoint-vect,alongX-pPoint,normalVect);
			angles.Add(sAngle);
		}
		int numTemp = angles.Count;
		while(numTemp>0)
		{
			float minAngle = 370;
			int indexAngle = -1;
			for (int i=0;i<angles.Count;i++)
			{
				if(minAngle>angles[i])
				{
					minAngle = angles[i];
					indexAngle = i;
				}
			}
			arrangedPoints.Add(endPts[indexAngle]);
			angles[indexAngle]=370;
			numTemp--;
		}
		return arrangedPoints;
	}
	private List<StandardPolygon> arrangeCounterClockwise(Geometry geo)
	{
		if(bShowLogs)
			Debug.Log("Entering arrangeCounterClockwise ");
		Line firstLine = geo.edges [0];
		Line secondLine = geo.edges [1];
		Vector3? cEndPt = firstLine.getCommonEndPoint (secondLine);
		if(cEndPt==null)
		{
			Debug.Log ("@@@@@@@@@@@@@@@@@@@ Should not be here");
			return null;
		}
		Vector3 commonEndPt = cEndPt.Value;
		Vector3 endPointThird = secondLine.GetOther(commonEndPt);
		bool counterClk = firstLine.CounterClockWise (firstLine.vertex[0],firstLine.vertex[1],endPointThird);
		Line begLine;
		if(counterClk)
		{
			begLine = firstLine;
		}
		else
		{
			begLine = new Line(firstLine.vertex[1],firstLine.vertex[0]);
		}
		List<StandardPolygon> sdList = new List<StandardPolygon>();
		StandardPolygon sd = new StandardPolygon ();
		sd.addPoint (begLine.vertex [0]);
		sd.addPoint (begLine.vertex [1]);
		bool addedAnother = true;
		List<Line> allEdges = new List<Line> ();
		allEdges.AddRange (geo.edges);
		//begLine.name = "edge 1";
		//%%%%%%%%%%%%%%%%%%%%%%
		//begLine.DrawVector (allLineParent);
		int ff = 1;
		/*foreach(Line l2 in allEdges)
		{
			l2.name = "edge "+ff; 
			l2.DrawVector (allLineParent);
			ff++;
		}*/
		StandardPolygon sdTemp = new StandardPolygon();
		/*foreach(Line l2 in allEdges)
		{
			sdTemp.addPoint(l2.vertex[0]);
			sdTemp.addPoint(l2.vertex[1]);
		}
		Debug.Break ();*/
		//allEdges = removeLineFromList (allEdges, begLine);
		/////////////////////////////
		Line lToRemove = begLine;
		int numRuns = 0;
		while(allEdges.Count>1)
		{
			numRuns++;
			if(numRuns>500)
			{
				Debug.Log("##################### Broke due to more infinity loop #############################");
				break;
			}
			bool addedAnotherPt = false;
			//int beforeInt = allEdges.Count;
			if(lToRemove!=null)
				allEdges = removeLineFromList (allEdges,lToRemove);
			//int afterInt = allEdges.Count;
			/*if(beforeInt==afterInt)
			{
				Debug.Log("%%%%%%%%%%%%%%%%%%%% Line not deleted");
			}*/

			////////////////////////////////////Start Here///////////////////////////////////////////////
			List<Vector3> endPts = new List<Vector3>();
			foreach(Line l in allEdges)
			{
				cEndPt = l.getCommonEndPoint (begLine);
				if(cEndPt==null)
				{
					continue;
				}
				commonEndPt = cEndPt.Value;
				if(commonEndPt.Equals(begLine.vertex[0]))
				{
					continue;
				}
				//Now common end point is begline.vertex[1]
				endPointThird = l.GetOther(begLine.vertex[1]);
				endPts.Add(endPointThird);
			}
			endPts = arrangePointsFromPoint(begLine.vertex[1],endPts);
			if(endPts.Count<=0)
				Debug.Log("%%%%%%%%%%%%%%%%%%%% arrangePointsFromPoint gives 0 end points");
			lToRemove = new Line(begLine.vertex[1],endPts[0]);
			begLine = new Line(begLine.vertex[1],endPts[0]);
			addedAnotherPt = sd.addPoint(endPts[0]);
			//ff++;
			//lToRemove.name = "edge "+ff; 
			//lToRemove.DrawVector (allLineParent);
			/*if(ff==10)
			{
				Debug.Log("edge 10 length = "+Vector3.Distance(lToRemove.vertex[0],lToRemove.vertex[1]));
			}*/
			if(lToRemove!=null)
				allEdges = removeLineFromList (allEdges,lToRemove);
			lToRemove=null;
			
			if(!addedAnotherPt)
			{
				Debug.Log("%%%%%%%%%%%%%%%%%%%% Pt not added. Must have completed a polygon.");
				StandardPolygon sdPolyNew = sd.makeSubPolygon(endPts[0]);
				if(sdPolyNew!=null)
					sdList.Add(sdPolyNew);
				//sdList.Add(sd);
				//sd = new StandardPolygon ();
			}
			continue;
			/// ///////////////////////////////////End Here////////////////////////////////////////////////////

			lToRemove = null;

			foreach(Line l in allEdges)
			{
				cEndPt = l.getCommonEndPoint (begLine);
				if(cEndPt==null)
				{
					continue;
				}
				commonEndPt = cEndPt.Value;
				if(commonEndPt.Equals(begLine.vertex[0]))
				{
					continue;
				}
				//Now common end point is begline.vertex[1]
				endPointThird = l.GetOther(begLine.vertex[1]);
				lToRemove = l;
				begLine = new Line(begLine.vertex[1],endPointThird);
				addedAnotherPt = sd.addPoint(endPointThird);
				//lToRemove.DrawVector(allLineParent);
				if(lToRemove!=null)
					allEdges = removeLineFromList (allEdges,lToRemove);
				lToRemove=null;

				if(!addedAnotherPt && allEdges.Count>=2)
				{
					Debug.Log("%%%%%%%%%%%%%%%%%%%% Pt not added. Should have been added.");
					StandardPolygon sdPolyNew = sd.makeSubPolygon(endPointThird);
					sdList.Add(sdPolyNew);
				}
				break;
			}
			/*if(!addedAnotherPt)
			{
				sdList.Add(sd);
				sd = new StandardPolygon ();
				//break;
			}*/
		}
		sdList.Add(sd);
		/*foreach(Line l2 in allEdges)
		{
			l2.name = "edge "+ff;
			ff++;
			l2.DrawVector(allLineParent);
		}*/
		return sdList;
		/// //////////////////////////
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		/// 
		//while(geo.edges.Count>sd.getVertices ().Count)
		while(addedAnother)
		{
			addedAnother = false;

			foreach(Line l in allEdges)
			{
				if(!l.Equals(begLine))
				{
					cEndPt = l.getCommonEndPoint (begLine);
					if(cEndPt==null)
					{
						continue;
					}
					commonEndPt = cEndPt.Value;
					if(commonEndPt==begLine.vertex[0])
					{
						continue;
					}
					endPointThird = l.GetOther(commonEndPt);
					begLine = new Line(commonEndPt,endPointThird);
					addedAnother = sd.addPoint(endPointThird);

					if(addedAnother)
					{
						if(bShowLogs)
						{
							Debug.Log("In arrangeCounterClockwise : added point. Count = "+sd.getVertices().Count);
						}
						int indx = sd.getVertices().Count;
						Line lTemp = new Line(sd.getVertices()[indx-2],sd.getVertices()[indx-1]);
						indx--;
						lTemp.name = "edge "+indx;
						//lTemp.DrawVector (allLineParent);
						if(indx==4 && allEdges.Count>2)
						{
							foreach(Line l2 in geo.edges)
							{
								if(l2.Equals(lTemp))
								{
									
									Debug.Log("%%%%%%%%%%%%%%%%%%%% The edge is in geometry");
									break;
								}
							}
						}
						allEdges = removeLineFromList (allEdges,l );
						break;
					}
					else
					{
						Debug.Log("In arrangeCounterClockwise : DID NOT add point.");
					}
				}
			}
		}
		Debug.Log ("allEdges.Count = " + allEdges.Count);
		//Till here added all points in standard polygon  in counterclockwise fashion.
		if (geo.edges.Count != sd.getVertices ().Count)
		{
			Debug.Log("geo.edges.Count = "+geo.edges.Count);
			Debug.Log ("@@@@@@@@@@@@@@@@@@@ Counter clockwise polygon is not complete");
		}
		/*foreach(Line l in geo.edges)
		{
			showPosOfPoint(l.vertex[0]);
			showPosOfPoint(l.vertex[1]);
		}*/
		//sd.removeDuplicates();
		/*foreach(Vector3 vect in sd.getVertices())
		{
			showPosOfPoint(vect);
		}*/
		if(bShowLogs)
		{
			Debug.Log("arranged CounterClockwise ");
		}

		return sdList;
	}
	public GameObject rectangleShow;
	void showPosOfPointRectangle(Vector3 pos,Color c)
	{
		if (float.IsNaN (pos.x) || float.IsNaN (pos.z))
			return;
		//GameObject sp = (GameObject)GameObject.Find ("StartPoint");
		//GameObject tempObj = (GameObject)GameObject.Instantiate (sp);
		GameObject tempObj = Instantiate(rectangleShow, pos, rectangleShow.transform.rotation) as GameObject;
		Vector3 lscale= tempObj.transform.localScale;
		float scalingTemp = 1.0f;

		if(currSceneName=="myCrash.unity" || currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			scalingTemp = 0.57f;//0.778f;
		}
		else if(currSceneName=="wastleLands2.unity")
		{
			scalingTemp = 0.6f;
		}
		else if(currSceneName=="testCase1.unity")
		{
			scalingTemp = 0.65f;
		}
		else if(currSceneName=="myScene1.unity")
		{
			scalingTemp = 0.6f;
		}

		lscale.x*=playerScaleForCurrent*scalingTemp;
		lscale.y*=playerScaleForCurrent*scalingTemp;
		lscale.z*=playerScaleForCurrent*scalingTemp;

		Renderer rend = tempObj.GetComponent<Renderer>();
		rend.material.color = c;
		rend.transform.localScale = lscale;
		tempObj.transform.position=pos;
		tempObj.transform.parent = allLineParent.transform;
		if(bJustTestCrashNow && currSceneName=="Crash.unity")
		{
			listShows.Add(tempObj);
		}
	}
	void showPosOfPointEnemySized(Vector3 pos,Color c)
	{
		if (float.IsNaN (pos.x) || float.IsNaN (pos.z))
			return;
		//GameObject sp = (GameObject)GameObject.Find ("StartPoint");
		//GameObject tempObj = (GameObject)GameObject.Instantiate (sp);
		GameObject tempObj = Instantiate(enemyPrefab, pos, enemyPrefab.transform.rotation) as GameObject;
		Vector3 lscale= tempObj.transform.localScale;

		lscale.x*=playerScaleForCurrent;
		lscale.y*=playerScaleForCurrent;
		lscale.z*=playerScaleForCurrent;
		//lscale.x*=10f;
		//lscale.y*=10f;
		//lscale.z*=10f;
		
		Renderer rend = tempObj.GetComponent<Renderer>();
		rend.material.color = c;
		rend.transform.localScale = lscale;
		tempObj.transform.position=pos;
		tempObj.transform.parent = allLineParent.transform;
	}
	void showPosOfPoint(Vector3 pos,Color c)
	{
		if (float.IsNaN (pos.x) || float.IsNaN (pos.z))
			return;
		//GameObject sp = (GameObject)GameObject.Find ("StartPoint");
		//GameObject tempObj = (GameObject)GameObject.Instantiate (sp);
		GameObject tempObj = Instantiate(pathSphere, pos, pathSphere.transform.rotation) as GameObject;
		Vector3 lscale= tempObj.transform.localScale;
		if(currSceneName=="myCrash.unity" || currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			lscale.x*=0.7f;
			lscale.y*=0.7f;
			lscale.z*=0.7f;
		}

		Renderer rend = tempObj.GetComponent<Renderer>();
		rend.material.color = c;
		rend.transform.localScale = lscale;
		tempObj.transform.position=pos;
		tempObj.transform.parent = allLineParent.transform;
	}
	List<int> applyEarClipping(/*Geometry shadowGeo,*/ List<Vector3> points)
	{
		Geometry shadowGeo = new Geometry ();
		for(int i=0;i<points.Count;i++)
		{
			shadowGeo.edges.Add(new Line(points[i],points[(i+1)%points.Count]));
		}
		List<int> newTriangles = new List<int>();
		if (points.Count <= 2)
			return newTriangles;
		if(points.Count==3)
		{
			newTriangles.Add(2);
			newTriangles.Add(1);
			newTriangles.Add(0);

			return newTriangles;
		}
		List<Vector3> copyPoints = new List<Vector3>();
		copyPoints.AddRange (points);
		if(bShowLogs)
			Debug.Log ("applyEarClipping " + copyPoints.Count);
		int numPts = copyPoints.Count;
		int numRuns = 0;
		while(true)
		{
			int itr=0;
			while(itr<numPts)
			{
				numRuns++;
				if(numPts<=3)
					break;
				if(numRuns>50)
				{

					Debug.Log("numRuns>50 ******* SHOULD NEVER BE HERE ******* ");
					Debug.Log("numRuns "+numRuns+") "+"numPts "+numPts+" itr "+itr);
					/*foreach(Vector3 pt in copyPoints)
					{
						showPosOfPoint(pt);
					}*/
					return newTriangles;
				}
				Line tmpLine = new Line(copyPoints[itr],copyPoints[(itr+2)%numPts]);
				//first check:mid point inside
				if(shadowGeo.PointInside(tmpLine.MidPoint()))
				{
					if(bShowLogs)
					{
						Debug.Log("MidPoint Inside");
						Debug.Log("numRuns "+numRuns+") "+"numPts "+numPts+" itr "+itr);
					}
					//second check
					bool bIntersects=false;
					foreach(Line edge in shadowGeo.edges)
					{
						if(edge.CommonEndPoint(tmpLine))
							continue;
						if(edge.LineIntersectMuntacEndPt(tmpLine)!=0)
						{
							bIntersects=true;
							break;
						}
					}
					if(!bIntersects)
					{
						newTriangles.Add(points.IndexOf(copyPoints[itr]));
						newTriangles.Add(points.IndexOf(copyPoints[(itr+1)%numPts]));
						newTriangles.Add(points.IndexOf(copyPoints[(itr+2)%numPts]));
						copyPoints.RemoveAt((itr+1)%numPts);
						if((itr+1)%numPts==0)
						{
							itr--;
						}
						numPts--;
						if(bShowLogs)
						{
							Debug.Log("Does not intersect");
							Debug.Log("numRuns "+numRuns+") "+"numPts "+numPts+" itr "+itr);
						}


					}
					else
					{
						itr++;
					}
				}
				else
				{
					itr++;
				}
			}
			if(numPts<=3)
				break;
		}
		newTriangles.Add(points.IndexOf(copyPoints[0]));
		newTriangles.Add(points.IndexOf(copyPoints[1]));
		newTriangles.Add(points.IndexOf(copyPoints[2]));

		if(bShowLogs)
			Debug.Log("applyEarClipping returning");
		return newTriangles;
	}
	private GameObject allLineParent;
	private Geometry findCurrentShadowPolygon(Vector3 pt,int Indx)
	{
		List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [Indx]];
		foreach(Geometry geo in shadowPolyTemp)
		{
			if(geo.PointInside(pt))
				return geo;
		}
		return null;
	}
	private bool pointInShadow(Vector3 pt,int Indx)
	{
		if(bMultiplePaths)
		{
			return pointInShadowMultiplePaths(pt,Indx);
		}
		if (Indx >= pathPoints.Count || Indx < 0)
		{
			//Debug.LogError(Indx);
			return false;
		}
		bool bTestAllPoints = true;
		//Geometry visibleGeoTemp = (Geometry)hVisiblePolyTable[pathPoints [Indx]];
		//List<VisibleTriangles> listVT = ((List<VisibleTriangles>)(hVisibleTrianglesTable[pathPoints[Indx]]));
		List<VisibleTriangles> listVT = ((List<VisibleTriangles>)(hVisibleTrianglesTable[Indx]));
		List<Vector3> pointListToTest = new List<Vector3>();
		//Debug.Log (radius_enemy + " radius_enemy");
		if(bTestAllPoints)
		{
			//8 points tested
			pointListToTest.Add (pt);
			pointListToTest.Add(new Vector3(pt.x+radius_enemy,pt.y,pt.z));
			pointListToTest.Add(new Vector3(pt.x-radius_enemy,pt.y,pt.z));
			pointListToTest.Add(new Vector3(pt.x,pt.y,pt.z+radius_enemy));
			pointListToTest.Add(new Vector3(pt.x,pt.y,pt.z-radius_enemy));
			List<int> listAngleVars = new List<int>();
			listAngleVars.Add(45);
			listAngleVars.Add(-45);
			listAngleVars.Add(135);
			listAngleVars.Add(-135);
			foreach(int angleVar in listAngleVars)
			{
				Vector3 vecSel = new Vector3();
				vecSel.x = pt.x + radius_enemy*Mathf.Cos(angleVar* Mathf.Deg2Rad);
				vecSel.y = pt.y;
				vecSel.z = pt.z + radius_enemy*Mathf.Sin(angleVar* Mathf.Deg2Rad);
				pointListToTest.Add(vecSel);
			}
		}
		else
		{
			pointListToTest.Add (pt);
		}
		foreach(Vector3 pt1 in pointListToTest)
		{
			/*Vector2 pt2Temp = new Vector2(-1f,-1f);
			if(h_mapPtToIndx.ContainsKey(h_mapPtToIndx))
				pt2Temp = (Vector2)h_mapPtToIndx[pt1];*/

			//If outside the boundary, return false
			if (!mapBG.PointInside (pt1))
			{
				//Debug.Log("("+pt2Temp.x+" , "+pt2Temp.y+")"+"point not In Shadow. "+pt1+" is outside the boundary");
				return false;
			}
			//If inside any obstacles, return false
			foreach(Geometry geo in globalPolygon)
			{
				if(geo.PointInside(pt1))
				{
					//Debug.Log("("+pt2Temp.x+" , "+pt2Temp.y+")"+"point not In Shadow. "+pt1+" is inside an obstacle");
					return false;
				}
			}
			/*foreach(VisibleTriangles vt in listVT)
			{
				if(vt.PointInside(pt1))
				{
					return false;
				}
			}*/
			bool res9 = AnotherCheckIfVisible(pathPoints[Indx],pt1,listVT);
			if(res9)
			{
				return false;
			}
			/*if(visibleGeoTemp.PointInside(pt1))
			{
				return false;
			}*/
		}
		/*foreach(Vector3 pt1 in pointListToTest)
			showPosOfPoint(pt1,Color.yellow);*/
		return true;
	}
	private bool AnotherCheckIfVisible(Vector3 pathPointCurr,Vector3 pt1,List<VisibleTriangles> listVT)
	{
		List<Line> listEdges = new List<Line> ();
		foreach(VisibleTriangles vt in listVT)
		{
			listEdges.Add(new Line(vt.pt2,vt.pt3));
		}
		Line lineToTest = new Line (pathPointCurr, pt1);
		foreach(Line l in listEdges)
		{
			if(lineToTest.LineIntersectMuntacEndPt(l)>0)
				return false;
		}
		return true;
	}
	private bool pointInShadow_Old(Vector3 pt,int Indx)
	{
		if (Indx >= pathPoints.Count || Indx < 0)
		{
			//Debug.LogError(Indx);
			return false;
		}
		//If inside any obstacles, return false
		foreach(Geometry geo in globalPolygon)
		{
			if(geo.PointInside(pt))
			{
				//Debug.Log("point not In Shadow. "+pt+" is inside an obstacle");
				return false;
			}
		}
		//If outside the boundary, return false
		if (!mapBG.PointInside (pt))
		{
			//Debug.Log("point not In Shadow. "+pt+" is outside the boundary");
			return false;
		}
		bool bTestAllPoints = false;
		List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [Indx]];
		if(bTestAllPoints)
		{
			List<Vector3> pointListToTest = new List<Vector3>();
			pointListToTest.Add (pt);
			pointListToTest.Add(new Vector3(pt.x+radius_enemy,pt.y,pt.z));
			pointListToTest.Add(new Vector3(pt.x-radius_enemy,pt.y,pt.z));
			pointListToTest.Add(new Vector3(pt.x,pt.y,pt.z+radius_enemy));
			pointListToTest.Add(new Vector3(pt.x,pt.y,pt.z-radius_enemy));
			foreach(Vector3 pt1 in pointListToTest)
			{
				foreach(Geometry geo in shadowPolyTemp)
				{
					if(!geo.PointInside(pt1))
						return false;
				}
			}
			return true;
		}
		else
		{
			foreach(Geometry geo in shadowPolyTemp)
			{
				if(geo.PointInside(pt))
				{
					return true;
				}
			}
			//Debug.Log("point not In Shadow. "+pt+" is outside shadow Polygon");
			return false;
		}
}
	//Key = path point (Vector3), Value = 2D array of 1's represent visible, 0's shadows, 2's for inside obstacle ...
	Hashtable h_discreteShadows = new Hashtable();
	//Key = (i,j) , Value = Vector3. i,j are indices corresponding to 2D arrays in h_discreteShadows
	Hashtable h_mapIndxToPt = new Hashtable();
	Hashtable h_mapPtToIndx = new Hashtable();
	//Func: fills h_discreteShadows and h_mapIndxToPt according to step and pathPoints
	int discretePtsX = -1;
	int discretePtsZ = -1;
	private void createDiscreteMap()
	{
		int Indx = 0;
		while(Indx<pathPoints.Count)
		{
			if(h_discreteShadows.ContainsKey(Indx))
			{
				Indx++;
				continue;
			}
			List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [Indx]];
			sbyte[,] shadowArray = new sbyte[discretePtsX,discretePtsZ];

			float radius_hiddenSphere = ((SphereCollider)hiddenSphere.GetComponent<Collider>()).radius*((SphereCollider)hiddenSphere.GetComponent<Collider>()).transform.lossyScale.x;
			int j1=0;	
			for(float j=m_minX;j<m_maxX && j1<discretePtsX;j+=m_step)
			{
				int k1=0;
				for(float k=m_minZ;k<m_maxZ && k1<discretePtsZ;k+=m_step)
				{
					Vector3 pt = new Vector3(j,1,k);

					if(pointInShadow(pt,Indx) && !Physics.CheckSphere(pt,radius_hiddenSphere))
					{
						shadowArray[j1,k1]=0;
					}
					else if(CheckIfInsidePolygon(pt))
					{
						shadowArray[j1,k1]=2;
					}
					else
					{
						shadowArray[j1,k1]=1;
					}
					k1++;
				}
				j1++;
			}
			h_discreteShadows.Add(Indx,shadowArray);
			Indx++;
		}
	}
	private void displayStrategicPoints (int Indx)
	{
		Geometry visiblePolyTemp = (Geometry)hVisiblePolyTable [pathPoints [Indx]];
		if(visiblePolyTemp!=null)
		{
			List<Line> allEdges = new List<Line> ();
			allEdges.AddRange (visiblePolyTemp.edges);
			int ff = 1;
			foreach(Line l2 in allEdges)
			{
				l2.name = "edge "+ff; 
				l2.DrawVector (allLineParent);
				ff++;
			}
		}
		float minX = mapBoundary[0].x;
		float minZ = mapBoundary[0].z;
		float maxX = mapBoundary[0].x;
		float maxZ = mapBoundary[0].z;
		for(int i=1;i<4;i++)
		{
			if(minX>mapBoundary[i].x)
			{
				minX=mapBoundary[i].x;
			}
			if(minZ>mapBoundary[i].z)
			{
				minZ=mapBoundary[i].z;
			}
			if(maxX<mapBoundary[i].x)
			{
				maxX=mapBoundary[i].x;
			}
			if(maxZ<mapBoundary[i].z)
			{
				maxZ=mapBoundary[i].z;
			}
		}
		List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [Indx]];
		float step = 0.1f;
		for(int i=0;i<shadowPolyTemp.Count;i++)
		{
			////////////////////////////////////////////
			//List<StandardPolygon> sdList = arrangeCounterClockwise(shadowPolyTemp[i]);
			/*List<Line> allEdges = new List<Line> ();
			allEdges.AddRange (shadowPolyTemp[i].edges);
			int ff = 1;
			foreach(Line l2 in allEdges)
			{
				l2.name = "edge "+ff; 
				l2.DrawVector (allLineParent);
				ff++;
			}*/

			////////////////////////////////////////////;
			float radius_hiddenSphere = ((SphereCollider)hiddenSphere.GetComponent<Collider>()).radius*((SphereCollider)hiddenSphere.GetComponent<Collider>()).transform.lossyScale.x;
			//Debug.Log ("radius" + radius_hiddenSphere);
			hiddenSphereList = new List<GameObject> ();
			//Foreach point:
			for(float j=minX;j<maxX;j+=step)
			{
				for(float k=minZ;k<maxZ;k+=step)
				{
					Vector3 pt = new Vector3(j,1,k);
					if(pointInShadow(pt,Indx) && !Physics.CheckSphere(pt,radius_hiddenSphere))
					{
						GameObject clone1 = (GameObject)Instantiate(hiddenSphere);
						clone1.transform.position = pt;
						hiddenSphereList.Add(clone1);
					}
				}
			}
		}
	}
	class VisibleTriangulation
	{
		List<Vector3> points;
		List<int> newTriangles;
		public void setPoints(List<Vector3> pts)
		{
			points = pts;
		}
		public void setTriangles(List<int> newT)
		{
			newTriangles = newT;
		}
		public List<Vector3> getPoints()
		{
			return points;
		}
		public List<int> getTriangles()
		{
			return newTriangles;
		}
	}
	private List<int> reverseTriangels(List<int> newTriangles)
	{
		List<int> newTrianglesReversed = new List<int> ();
		for(int i=0;i<newTriangles.Count-3;i+=3)
		{
			newTrianglesReversed.Add(newTriangles[i+2]);
			newTrianglesReversed.Add(newTriangles[i+1]);
			newTrianglesReversed.Add(newTriangles[i]);
		}
		return newTrianglesReversed;
	}
	List<int> triangulateVisible(List<Vector3> points)
	{
		List<int> newTriangles = new List<int>();
		int i = 1;
		for(i=1;i<points.Count-1;i++)
		{
			newTriangles.Add(0);
			newTriangles.Add(i);
			newTriangles.Add(i+1);
		}
		newTriangles.Add(0);
		newTriangles.Add(points.Count-1);
		newTriangles.Add(1);

		/*newTriangles.Add(0);
		newTriangles.Add(points.Count);
		newTriangles.Add(1);*/

		return newTriangles;
	}
	Hashtable h_visible_Star_Triangles = new Hashtable ();
	private void createVisibleTriangulation()
	{
		int Indx=0;
		while(Indx<pathPoints.Count)
		{
			Geometry visiblePolyTemp = (Geometry)hVisiblePolyTable [pathPoints [Indx]];

			//List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [Indx]];
			List<int> newTriangles;
			List<StandardPolygon> sdList = arrangeCounterClockwise(visiblePolyTemp);
			List<VisibleTriangulation> llist = new List<VisibleTriangulation>();
			//for(int i=0;i<shadowPolyTemp.Count;i++)
			{
				//List<StandardPolygon> sdList = arrangeCounterClockwise(shadowPolyTemp[i]);
				
				foreach(StandardPolygon sd in sdList)
				{
					List<Vector3> points = sd.getVertices();
					//newTriangles = applyEarClipping(points);

					List<Vector3> pointsTemp = new List<Vector3>();
					pointsTemp.Add(pathPoints[Indx]);
					pointsTemp.AddRange(points);
					points = pointsTemp;

					newTriangles = triangulateVisible(points);
					List<int> newTrianglesReversed = reverseTriangels(newTriangles);
					//newTriangles.AddRange(newTrianglesReversed);
					//newTriangles = newTrianglesReversed;

					VisibleTriangulation tempVT = new VisibleTriangulation();
					tempVT.setPoints(points);
					tempVT.setTriangles(newTriangles);
					llist.Add(tempVT);
				}
			}
			h_visible_Star_Triangles.Add(Indx,llist);
			Indx++;
		}
	}
	private void displayShadowMeshes(int Indx)
	{
		/*Geometry visiblePolyTemp = (Geometry)hVisiblePolyTable [pathPoints [Indx]];

		List<StandardPolygon> sdList = arrangeCounterClockwise(visiblePolyTemp);
		Debug.Log("sdList.Count = "+sdList.Count);*/
		/*Geometry visiblePolyTemp = (Geometry)hVisiblePolyTable [pathPoints [Indx]];
		foreach(Line l in visiblePolyTemp.edges)
		{
			l.DrawVector(allLineParent);
		}*/


		List<int> newTriangles;
		foreach(GameObject tempObj in shadowMeshes)
		{
			GameObject.Destroy(tempObj);
		}
		shadowMeshes.Clear ();
		List<VisibleTriangulation> llist = (List<VisibleTriangulation>)h_visible_Star_Triangles[Indx];

		foreach(VisibleTriangulation VT in llist)
		{
			//Debug.Log("displayShadowMeshes Index = "+Indx);
			/*Debug.Log("sd.Count = "+sd.getVertices().Count);
			List<Vector3> points = sd.getVertices();
			newTriangles = applyEarClipping(points);*/
			List<Vector3> points = VT.getPoints();
			Debug.Log("VT.getPoints() = "+VT.getPoints().Count);
			Debug.Log("VT.getTriangles() = "+VT.getTriangles().Count);
			newTriangles = VT.getTriangles();
			shadowMeshes.Add(new GameObject("ShadowMesh"));
			MeshFilter filter = shadowMeshes[shadowMeshes.Count-1].AddComponent<MeshFilter>();

			MeshRenderer meshRenderer = shadowMeshes[shadowMeshes.Count-1].AddComponent<MeshRenderer>();
			Material material = mat;
			if(material==null)
			{
				Debug.Log("material not found");
			}
			meshRenderer.material = material;
			
			Mesh mesh = filter.sharedMesh;
			if(mesh==null)
			{
				mesh = new Mesh();
				filter.sharedMesh = mesh;
			}
			Vector2[] uvs = new Vector2[points.Count];
			
			//shadowMeshes[shadowMeshes.Count-1].transform.position = new Vector3(0,1,0);
			
			int i2 = 0;
			while (i2 < uvs.Length) 
			{
				uvs[i2] = new Vector2(points[i2].x, points[i2].z);
				i2++;
			}
			
			Vector3[] normals = new Vector3[points.Count];
			for (i2 = 0; i2 < normals.Length; i2++) 
			{
				normals[i2] = Vector3.up;
			}
			
			
			mesh.vertices = points.ToArray();
			mesh.uv = uvs;
			mesh.triangles = newTriangles.ToArray();
			mesh.normals=normals;
			
			filter.mesh = mesh;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			//newTriangles.Clear();
		}
	}
	/*
	private void displayShadowMeshes_Old(int Indx)
	{
		//get the shadow polygons
		if(bShowLogs)
			Debug.Log("displayShadowMeshes = " + Indx);

		List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [Indx]];
		//List<Vector3> newVertices = new List<Vector3>();
		List<int> newTriangles;// = new List<int>();
		//For each shadow polygon


		for(int i=0;i<shadowPolyTemp.Count;i++)
		{
			Geometry shadowGeo = shadowPolyTemp[i];
			if(bShowLogs)
				Debug.Log("shadowGeo = "+shadowGeo.edges);
			Geometry geo = consolidateShadowPolygon (shadowGeo);

			List<StandardPolygon> sdList = arrangeCounterClockwise(geo);
			Debug.Log("sdList.Count = "+sdList.Count);
			foreach(StandardPolygon sd in sdList)
			{
				Debug.Log("sd.Count = "+sd.getVertices().Count);
			List<Vector3> points = sd.getVertices();
			
			newTriangles = applyEarClipping(points);
			/////////////////Just for debugging////////////////////
			for(int counter=0;counter<points.Count;counter++)
			{
				Line l = new Line(points[counter],points[(counter+1)%points.Count]);
				l.name = "edge "+counter;
				l.DrawVector (allLineParent);
			}
			/////////////////END: Just for debugging////////////////////
			if(bShowLogs)
			{
				Debug.Log("newTriangles = "+newTriangles.Count);
				Debug.Log("points = "+points.Count);
			}

			shadowMeshes.Add(new GameObject("ShadowMesh"));
			MeshFilter filter = shadowMeshes[shadowMeshes.Count-1].AddComponent<MeshFilter>();
			//shadowMeshes[shadowMeshes.Count-1].GetComponent<MeshFilter>();
			//filter.sharedMesh = mesh;
			MeshRenderer meshRenderer = shadowMeshes[shadowMeshes.Count-1].AddComponent<MeshRenderer>();
			
			Material material = mat;//Resources.Load("Shadow", typeof(Material)) as Material;
			if(material==null)
			{
				Debug.Log("material not found");
			}
			meshRenderer.material = material;
			
			Mesh mesh = filter.sharedMesh;
			if(mesh==null)
			{
				mesh = new Mesh();
				filter.sharedMesh = mesh;
			}
			Vector2[] uvs = new Vector2[points.Count];

			//shadowMeshes[shadowMeshes.Count-1].transform.position = new Vector3(0,1,0);

			int i2 = 0;
			while (i2 < uvs.Length) 
			{
				uvs[i2] = new Vector2(points[i2].x, points[i2].z);
				i2++;
			}
			
			Vector3[] normals = new Vector3[points.Count];
			for (i2 = 0; i2 < normals.Length; i2++) 
			{
				normals[i2] = Vector3.up;
			}
			
			
			mesh.vertices = points.ToArray();
			mesh.uv = uvs;
			mesh.triangles = newTriangles.ToArray();
			mesh.normals=normals;
			
			filter.mesh = mesh;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			//newTriangles.Clear();
			}
		}
		//bCallComplete=true;
	}
*/
	int shadowCounter=1;
	private void showShadowMesh ()
	{
		Debug.Log("Creating shadows "+shadowCounter++);
		List<Vector3> newVertices = new List<Vector3>();
		Vector2[] newUV;
		List<Line> newLines = new List<Line> ();
		List<Line> allLines = new List<Line> ();
		List<int> newTriangles = new List<int>();

		List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [startIndex]];
		for(int i=0;i<shadowPolyTemp.Count;i++)
		{
			shadowMeshes.Add(new GameObject("ShadowMesh"));
			MeshFilter filter = shadowMeshes[shadowMeshes.Count-1].AddComponent<MeshFilter>();
			//shadowMeshes[shadowMeshes.Count-1].GetComponent<MeshFilter>();
			//filter.sharedMesh = mesh;
			MeshRenderer meshRenderer = shadowMeshes[shadowMeshes.Count-1].AddComponent<MeshRenderer>();

			Material material = mat;//Resources.Load("Shadow", typeof(Material)) as Material;
			if(material==null)
			{
				Debug.Log("material not found");
			}
			meshRenderer.material = material;

			Mesh mesh = filter.sharedMesh;
			if(mesh==null)
			{
				mesh = new Mesh();
				filter.sharedMesh = mesh;
			}
			//mesh.Clear();
			//foreach(Geometry geo in shadowPolyTemp)
			//{
			foreach(Line edge in shadowPolyTemp[i].edges)
			{
				//May contain error as same point can be represented by two points which are quite near.
				if(!ListContainsPoint(newVertices,edge.vertex[0]))
				{
					newVertices.Add(edge.vertex[0]);
				}
				if(!ListContainsPoint(newVertices,edge.vertex[1]))
				{
					newVertices.Add(edge.vertex[1]);
				}
			}
			//}
			// Creating diagonals as newLines
			for(int j=0;j<newVertices.Count;j++)
			{
				for(int k=0;k<newVertices.Count;k++)
				{
					int diff = Mathf.Abs(j-k);
					//ignoring neighbours
					if(diff==0 || diff==1 || newVertices.Count%diff==1)
						continue;
					Line tmpLine = new Line(newVertices[j],newVertices[k]);
					//first check:mid point inside
					if(shadowPolyTemp[i].PointInside(tmpLine.MidPoint()))
					{
						//second check
						bool bIntersects=false;
						foreach(Line edge in shadowPolyTemp[i].edges)
						{
							if(edge.CommonEndPoint(tmpLine))
								continue;
							if(edge.LineIntersectMuntacEndPt(tmpLine)!=0)
							{
								bIntersects=true;
								break;
							}
						}
						if(!bIntersects)
						{
							newLines.Add(new Line(newVertices[j],newVertices[k]));
						}
					}
				}
			}
			allLines.AddRange(shadowPolyTemp[i].edges);
			allLines.AddRange(newLines);

			foreach(Line l in allLines)
			{
				//Debug.Log("From "+l.vertex[0]+" to "+l.vertex[1]);
				//Debug.DrawLine(l.vertex[0],l.vertex[1]);
				l.DrawLine(Color.red);

					GameObject pathObj;
					pathObj = Instantiate(pathSphere, 
				                      l.vertex[0], 
					                      pathSphere.transform.rotation) as GameObject;
				pathObj = Instantiate(pathSphere, 
				                      l.vertex[1], 
				                      pathSphere.transform.rotation) as GameObject;
				
			}



			//Creating triangles using diagonals
			foreach(Line diagonal in newLines)
			{
				foreach(Line anyLine in allLines)
				{
					if(diagonal.Equals(anyLine))
						continue;
					if(diagonal.CommonEndPoint(anyLine))
					{
						Vector3? commonEndPtTemp = diagonal.getCommonEndPoint(anyLine);
						if(commonEndPtTemp==null)
							Debug.Log ("Should not be here");
						Vector3 commonEndPt = commonEndPtTemp.Value;
						Vector3 endPoint1 = diagonal.GetOther(commonEndPt);
						Vector3 endPoint2 = anyLine.GetOther(commonEndPt);
						foreach(Line anyline2 in allLines)
						{
							if(anyline2.PointOnLine(endPoint1) && anyline2.PointOnLine(endPoint2))
							{
								/*if(ListContainsPoint(newVertices,commonEndPt))
								{
									if(newVertices.IndexOf(commonEndPt)<0)
									{
										Debug.Log(commonEndPt+" is inside list but index is not identified");
									}
								}
								if(ListContainsPoint(newVertices,endPoint1))
								{
									if(newVertices.IndexOf(endPoint1)<0)
									{
										Debug.Log(endPoint1+" is inside list but index is not identified");
									}
								}
								if(ListContainsPoint(newVertices,endPoint2))
								{
									if(newVertices.IndexOf(endPoint2)<0)
									{
										Debug.Log(endPoint2+" is inside list but index is not identified");
									}
								}
								*/
								//Debug.Log(newVertices.IndexOf(commonEndPt));
								//Debug.Log(newVertices.IndexOf(endPoint1));
								//Debug.Log(newVertices.IndexOf(endPoint2));
								newTriangles.Add(IndexInList(newVertices,commonEndPt));
								newTriangles.Add(IndexInList(newVertices,endPoint1));
								newTriangles.Add(IndexInList(newVertices,endPoint2));
								//newTriangles.Add(newVertices.IndexOf(commonEndPt));
								//newTriangles.Add(newVertices.IndexOf(endPoint1));
								//newTriangles.Add(newVertices.IndexOf(endPoint2));
							}
						}
					}
				}
			}
			/*for(int itr = 0;itr<newTriangles.Count-2;itr+=3)
			{
				Debug.DrawLine(newVertices[newTriangles[itr]],newVertices[newTriangles[itr+1]]);
				Debug.DrawLine(newVertices[newTriangles[itr+1]],newVertices[newTriangles[itr+2]]);
				Debug.DrawLine(newVertices[newTriangles[itr+2]],newVertices[newTriangles[itr]]);
			}*/

			Vector2[] uvs = new Vector2[newVertices.Count];
			int i2 = 0;

			shadowMeshes[shadowMeshes.Count-1].transform.position = new Vector3(0,1,0);//newVertices[0];
			/*for(i2=0;i2<newVertices.Count;i2++)
			{
				newVertices[i2]-=shadowMeshes[shadowMeshes.Count-1].transform.position;
			}*/
			i2 = 0;
			while (i2 < uvs.Length) 
			{
				uvs[i2] = new Vector2(newVertices[i2].x, newVertices[i2].z);
				i2++;
			}

			Vector3[] normals = new Vector3[newVertices.Count];
			for (i2 = 0; i2 < normals.Length; i2++) 
			{
				normals[i2] = Vector3.up;
			}


			mesh.vertices = newVertices.ToArray();
			mesh.uv = uvs;
			mesh.triangles = newTriangles.ToArray();
			mesh.normals=normals;

			filter.mesh = mesh;
			mesh.RecalculateNormals();
			mesh.RecalculateBounds();
			newTriangles.Clear();
			allLines.Clear();
			newLines.Clear();
			newVertices.Clear();
		}
		Debug.Break();

	}
	//float lastTimeUpdateCalled=Time.time;
	//int lastUpdateCalled=100;
	//System.DateTime lastTimeUpdateCalled = System.DateTime.Now;
	bool AnalyzeNearestPathPoint()
	{
		/*lastUpdateCalled--;
		if(lastUpdateCalled==0)
		{
			lastUpdateCalled=100;
		}
		else 
			return false;*/
		/*if(Time.time-lastTimeUpdateCalled>1.0f)
			lastTimeUpdateCalled = Time.time;
		else
			return false;*/
		float dist=1000;
		int index = -1;
		Vector3 mousePos = camObj.ScreenToWorldPoint(Input.mousePosition);
		mousePos.y=1;
		for(int i=0;i<pathPoints.Count;i++)
		{
			float distTemp = Vector3.Distance(pathPoints[i],mousePos);
			if(distTemp<dist)
			{
				dist=distTemp;
				index=i;
			}
		}
		//Debug.Log ("Found dist " + dist);
		if (dist > 0.05)
			return false;
		Debug.Log ("Found index " + index);


		displayShadowMeshes(index);
		return true;
	}
	bool bNearBy = false;
	bool bCallComplete=true;

	int startIndex = -1;
	int endIndex = -1;
	void IdentifyGoodHidingSpots ()
	{
		if (!b_ShowBoundbox)
			return;
		if (startIndex == -1)
			return;
		List<Line> hiddenLines = new List<Line> ();
		//ForEach first path point:
		//Identify lines behind which to hide
		List<Geometry> shadowPolyTemp = (List<Geometry>)hTable [pathPoints [startIndex]];
		foreach(Geometry geo in shadowPolyTemp)
		{
			foreach(Line l in geo.edges)
			{
				List<Vector3> pair = l.PointsOnEitherSide(0.02f);

				int ct_insideObstacle=0;
				//int ct_insideBoundary=0;
				foreach(Vector3 pt in pair)
				{
					foreach(Geometry g in globalPolygon)
					{
						if(g.PointInside(pt))
						{
							ct_insideObstacle++;
						}
					}
				}
				if(ct_insideObstacle==1)
				{
					hiddenLines.Add(l);
				}
			}
		}
		/* Used for removing lines which are open towards path point
		currIndex = 0;
		foreach(Line l in hiddenLines)
		{
			Vector3 midPt = l.MidPoint();
			Vector3 tempPt = Vector3.MoveTowards(midPt,pathPoints[startIndex],0.1f);
			bool b_insideObs=false;
			foreach(Geometry g in globalPolygon)
			{
				if(g.PointInside(tempPt))
				{
					b_insideObs=true;
				}
			}
			if(!b_insideObs)
			{
				hiddenLines[currIndex]=null;
			}
			
			currIndex++;
		}
		*/
		hiddenLines.RemoveAll(item=>item==null);
		float radius_hiddenSphere = ((SphereCollider)hiddenSphere.GetComponent<Collider>()).radius*((SphereCollider)hiddenSphere.GetComponent<Collider>()).transform.lossyScale.x;
		//Debug.Log ("radius" + radius_hiddenSphere);
		hiddenSphereList = new List<GameObject> ();
		//Foreach line:
		foreach(Line l in hiddenLines)
		{
			Vector3 towardsVect=l.vertex[0];
			while(towardsVect!=l.vertex[1])
			{
				Vector3 previous = towardsVect;
				towardsVect = Vector3.MoveTowards(previous,l.vertex[1],radius_hiddenSphere+0.01f);
				Line tempLine = new Line(previous,towardsVect);
				List<Vector3> pair = tempLine.PointsOnEitherSide(radius_hiddenSphere+0.01f);
				if(!Physics.CheckSphere(pair[0],radius_hiddenSphere))
				{
					GameObject clone1 = (GameObject)Instantiate(hiddenSphere);
					clone1.transform.position = pair[0];
					hiddenSphereList.Add(clone1);
				}
				if(!Physics.CheckSphere(pair[1],radius_hiddenSphere))
				{
					GameObject clone1 = (GameObject)Instantiate(hiddenSphere);
					clone1.transform.position = pair[1];
					hiddenSphereList.Add(clone1);
				}
			}
			////// move over line, identify point beside line inside shadow polygon
			////// , make abstract area and check if all points on hidden sphere fits in all shadow
		}
		List<Vector3> circumPoints = new List<Vector3>();
		for(int k=0;k<hiddenSphereList.Count;k++)
		{
			bool sphereFound=false;
			circumPoints.Clear();
			circumPoints.Add(new Vector3(hiddenSphereList[k].transform.position.x,1,hiddenSphereList[k].transform.position.z));
			//circumPoints.Add(new Vector3(hiddenSphereList[k].transform.position.x-radius_hiddenSphere,1,hiddenSphereList[k].transform.position.z));
			//circumPoints.Add(new Vector3(hiddenSphereList[k].transform.position.x+radius_hiddenSphere,1,hiddenSphereList[k].transform.position.z));
			//circumPoints.Add(new Vector3(hiddenSphereList[k].transform.position.x,1,hiddenSphereList[k].transform.position.z-radius_hiddenSphere));
			//circumPoints.Add(new Vector3(hiddenSphereList[k].transform.position.x,1,hiddenSphereList[k].transform.position.z+radius_hiddenSphere));
			for(int i=startIndex;i<=endIndex;i++)
			{
				shadowPolyTemp = (List<Geometry>)hTable [pathPoints [i]];
				int insideCounterTemp=0;
				sphereFound=false;
				foreach(Geometry geo in shadowPolyTemp)
				{
					insideCounterTemp=0;
					foreach(Vector3 vect in circumPoints)
					{
						if(geo.PointInside(vect))
						{
							insideCounterTemp++;
						}
					}
					//if(insideCounterTemp>0 && insideCounterTemp<4)
						//Debug.Log (insideCounterTemp);
					if(insideCounterTemp==circumPoints.Count)
					{
						sphereFound=true;
						break;
					}
				}
				if(!sphereFound)
				{
					GameObject.Destroy(hiddenSphereList[k]);
					hiddenSphereList[k]=null;
				}
			}
		}
		hiddenSphereList.RemoveAll(item=>item==null);
	}
	public bool floatCompare ( float a, float b )
	{
		if(currSceneName=="myCrash.unity" || currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			return Mathf.Abs (a - b) < eps4;
		}
		return Mathf.Abs (a - b) < eps;
	}
	private float eps = 0.01f;
	private float eps2 = 0.0001f;
	private float eps4 = 0.0001f;
	private float eps3 = 0.5f;
	public bool VectorApprox_Old ( Vector3 a, Vector3 b ){
		if( Mathf.Abs (a.x - b.x) < eps && Mathf.Abs (a.z - b.z) < eps )
		{
			//Debug.Log(Mathf.Abs (a.x - b.x) +"<"+ eps);
			//Debug.Log(Mathf.Abs (a.z - b.z) +"<"+ eps);
			return true;
		}
		else
			return false;
	}
	public bool VectorApprox ( Vector3 a, Vector3 b )
	{
		if(currSceneName=="myCrash.unity" || currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			if( Mathf.Abs (a.x - b.x) < eps4 && Mathf.Abs (a.z - b.z) < eps4 )
			{
				return true;
			}
			else
				return false;
		}
		if( Mathf.Abs (a.x - b.x) < eps2 && Mathf.Abs (a.z - b.z) < eps2 )
		{
			//Debug.Log(Mathf.Abs (a.x - b.x) +"<"+ eps);
			//Debug.Log(Mathf.Abs (a.z - b.z) +"<"+ eps);
			return true;
		}
		else
			return false;
	}
	public bool VectorApprox2 ( Vector3 a, Vector3 b )
	{
		if(currSceneName == "Crash.unity")
		{
			if( Mathf.Abs (a.x - b.x) < eps3 && Mathf.Abs (a.z - b.z) < eps3 )
			{
				return true;
			}
			else
				return false;
		}
		else if(currSceneName=="myCrash.unity" || currSceneName=="myCrash_Shorter.unity" || currSceneName=="myCrash_Shorter2.unity")
		{
			if( Mathf.Abs (a.x - b.x) < eps4 && Mathf.Abs (a.z - b.z) < eps4 )
			{
				return true;
			}
			else
				return false;
		}
		else
		{
			if( Mathf.Abs (a.x - b.x) < eps2 && Mathf.Abs (a.z - b.z) < eps2 )
			{
				//Debug.Log(Mathf.Abs (a.x - b.x) +"<"+ eps);
				//Debug.Log(Mathf.Abs (a.z - b.z) +"<"+ eps);
				return true;
			}
			else
				return false;
		}
	}
	public class VisibleTriangles
	{
		public Vector3 pt1;
		public Vector3 pt2;
		public Vector3 pt3;
		public static Material matTriangle;
		public static GameObject allChildParent;
		public static GameObject enemyPrefab;
		void showPosOfPointEnemySized(Vector3 pos,Color c)
		{
			if (float.IsNaN (pos.x) || float.IsNaN (pos.z))
				return;
			//GameObject sp = (GameObject)GameObject.Find ("StartPoint");
			//GameObject tempObj = (GameObject)GameObject.Instantiate (sp);
			GameObject tempObj = Instantiate(enemyPrefab, pos, enemyPrefab.transform.rotation) as GameObject;
			Vector3 lscale= tempObj.transform.localScale;
			//lscale.x*=10f;
			//lscale.y*=10f;
			//lscale.z*=10f;
			
			Renderer rend = tempObj.GetComponent<Renderer>();
			rend.material.color = c;
			rend.transform.localScale = lscale;
			tempObj.transform.position=pos;
			tempObj.transform.SetParent (allChildParent.transform);
		}
		public bool PointOnLine(Vector3 pt)
		{
			List<Line> edgesTriangle = new List<Line> ();
			edgesTriangle.Add (new Line (pt1, pt2));
			edgesTriangle.Add (new Line (pt2, pt3));
			edgesTriangle.Add (new Line (pt3, pt1));
			foreach(Line myLine in edgesTriangle)
			{
				if(myLine.PointOnLine_LessAccurate(pt))
				{
					return true;
				}
			}
			return false;
		}
		public bool PointInside( Vector3 pt )
		{
			if (PointOnLine (pt))
				return true;
			int extreme = 500;
			List<Line> lRayList = new List<Line> ();
			List<Vector3> pointListToTest = new List<Vector3>();
			List<int> listAngleVars = new List<int>();
			/*for(int itr=10;itr<360;itr+=30)
		{
			listAngleVars.Add(itr);
		}*/
			listAngleVars.Add(45);
			listAngleVars.Add(-45);
			listAngleVars.Add(90);
			listAngleVars.Add(-90);
			listAngleVars.Add(135);
			listAngleVars.Add(-135);
			listAngleVars.Add(0);
			listAngleVars.Add(180);
			foreach(int angleVar in listAngleVars)
			{
				Vector3 vecSel = new Vector3();
				vecSel.x = pt.x + extreme*Mathf.Cos(angleVar* Mathf.Deg2Rad);
				vecSel.y = pt.y;
				vecSel.z = pt.z + extreme*Mathf.Sin(angleVar* Mathf.Deg2Rad);
				pointListToTest.Add(vecSel);
			}
			//Line lray = new Line(pt, new Vector3(-100,1,-100)); 
			int count = 0;
			foreach(Vector3 vectVar in pointListToTest)
			{
				lRayList.Add (new Line (pt, vectVar));
			}
			
			//lRayList.Add (new Line (pt, new Vector3 (-extreme, 1,-extreme)));
			//lRayList.Add (new Line (pt, new Vector3 (extreme, 1, -extreme)));
			//lRayList.Add (new Line (pt, new Vector3 (extreme, 1, extreme)));
			//lRayList.Add (new Line (pt, new Vector3 (-extreme, 1, extreme)));
			List<Line> edgesTriangle = new List<Line> ();
			edgesTriangle.Add (new Line (pt1, pt2));
			edgesTriangle.Add (new Line (pt2, pt3));
			edgesTriangle.Add (new Line (pt3, pt1));
			int count1 = 0;
			float epsPtInside = 0.0001f;
			foreach(Line lray in lRayList)
			{
				count=0;
				foreach(Line myLine in edgesTriangle)
				{
					if( myLine.LineIntersectMuntacEndPt(lray) > 0 )
					{
						count++;
						//Check if the intersection point is on the polygon edge
						//Note: other checks tried but precision error kept coming up in cases
						Vector3 vtemp = myLine.GetIntersectionPoint(lray);
						if( Mathf.Abs( vtemp.x - pt.x ) < epsPtInside && Mathf.Abs(vtemp.z - pt.z) < epsPtInside )
							return false;
					}
				}
				if(count%2 != 1)
				{
					count1++;
					//return false;
				}
			}
			if (count1 >= lRayList.Count/2)
				return false;
			//return count%2 == 1; 
			return true;
		}
		private void DrawLine()
		{
			GameObject allLineParentChild = new GameObject();
			LineRenderer lineR = allLineParentChild.AddComponent<LineRenderer>();
			lineR.material = matTriangle;
			lineR.SetWidth(0.05f,0.05f);
			lineR.SetVertexCount(2);
			lineR.SetColors (Color.cyan, Color.cyan);

			Vector3 vertexPt = new Vector3(pt2.x,2.0f,pt2.z);
			lineR.SetPosition (0,vertexPt);
			vertexPt = new Vector3(pt3.x,2.0f,pt3.z);
			lineR.SetPosition (1, vertexPt);

			allLineParentChild.transform.SetParent (allChildParent.transform);

			Renderer rend = allLineParentChild.GetComponent<Renderer>();
			rend.material.color = Color.cyan;
			lineR.material.color = Color.cyan;
		}
		public void DrawTriangle()
		{
			//DrawLine ();
			/*GameObject GO_Triangle = new GameObject ();

			GO_Triangle.AddComponent<MeshFilter>();
			GO_Triangle.AddComponent<MeshRenderer>();
			Mesh mesh = GO_Triangle.GetComponent<MeshFilter>().mesh;
			mesh.Clear();
			mesh.vertices = new Vector3[] {pt1,pt2,pt3};
			mesh.uv = new Vector2[]
			{
				new Vector2(0f,0f),
				new Vector2(1f,0f),
				new Vector2(.5f, Mathf.Sin(Mathf.Deg2Rad * 45f) )
			};
			mesh.triangles = new int[] {0, 1, 2};
			MeshRenderer meshRenderer = GO_Triangle.GetComponent<MeshRenderer>();
			meshRenderer.material = matTriangle;
			Vector3[] normals = new Vector3[3]{Vector3.forward,Vector3.forward,Vector3.forward};
			mesh.normals = normals;
			*/
			/*Mesh mesh = new Mesh();
			int[] tris = new int[3]{0,1,2};
			Vector3[] verts = new Vector3[3];
			Vector2[] uvs = new Vector2[3];
			verts [0] = -Vector3.right + Vector3.up;
			verts [1] = Vector3.right + Vector3.up;
			verts [2] = -Vector3.right - Vector3.up;
			uvs [0] = new Vector2 (0.0f,1.0f);
			uvs [1] = new Vector2 (1.0f,1.0f);
			uvs [2] = new Vector2 (0.0f,0.0f);
			mesh.vertices = verts;
			mesh.triangles = tris;
			mesh.uv = uvs;
			mesh.RecalculateNormals ();
			GO_Triangle.AddComponent<MeshFilter> ().mesh = mesh;
			GO_Triangle.AddComponent<MeshRenderer>();*/
			{
					GameObject GO_Triangle = new GameObject ();
					GO_Triangle.AddComponent<MeshFilter> ();
					GO_Triangle.AddComponent<MeshRenderer> ();
					MeshRenderer meshRenderer = GO_Triangle.GetComponent<MeshRenderer> ();
					meshRenderer.material = matTriangle;
					MeshFilter filter = GO_Triangle.GetComponent<MeshFilter> ();
					Mesh mesh = filter.sharedMesh;
					if (mesh == null) {
							mesh = new Mesh ();
							filter.sharedMesh = mesh;
					}
					Vector2[] uvs = new Vector2[3];


					int i2 = 0;
					uvs [0] = new Vector2 (pt1.x, pt1.z);
					uvs [1] = new Vector2 (pt2.x, pt2.z);
					uvs [2] = new Vector2 (pt3.x, pt3.z);

					Vector3[] normals = new Vector3[3];
					normals [0] = Vector3.up;
					normals [1] = Vector3.up;
					normals [2] = Vector3.up;


					mesh.vertices = new Vector3[]{pt1,pt2,pt3};
					mesh.uv = uvs;
					mesh.triangles = new int[]{1,0,2};
					mesh.normals = normals;

					filter.mesh = mesh;
					mesh.RecalculateNormals ();
					mesh.RecalculateBounds ();

					GO_Triangle.transform.SetParent (allChildParent.transform);
			}
			{
				GameObject GO_Triangle = new GameObject ();
				GO_Triangle.AddComponent<MeshFilter> ();
				GO_Triangle.AddComponent<MeshRenderer> ();
				MeshRenderer meshRenderer = GO_Triangle.GetComponent<MeshRenderer> ();
				meshRenderer.material = matTriangle;
				MeshFilter filter = GO_Triangle.GetComponent<MeshFilter> ();
				Mesh mesh = filter.sharedMesh;
				if (mesh == null) {
					mesh = new Mesh ();
					filter.sharedMesh = mesh;
				}
				Vector2[] uvs = new Vector2[3];
				
				
				int i2 = 0;
				uvs [0] = new Vector2 (pt1.x, pt1.z);
				uvs [1] = new Vector2 (pt2.x, pt2.z);
				uvs [2] = new Vector2 (pt3.x, pt3.z);
				
				Vector3[] normals = new Vector3[3];
				normals [0] = Vector3.up;
				normals [1] = Vector3.up;
				normals [2] = Vector3.up;
				
				
				mesh.vertices = new Vector3[]{pt1,pt2,pt3};
				mesh.uv = uvs;
				mesh.triangles = new int[]{0,2,1};
				mesh.normals = normals;
				
				filter.mesh = mesh;
				mesh.RecalculateNormals ();
				mesh.RecalculateBounds ();
				
				GO_Triangle.transform.SetParent (allChildParent.transform);
			}
			{
				GameObject GO_Triangle = new GameObject ();
				GO_Triangle.AddComponent<MeshFilter> ();
				GO_Triangle.AddComponent<MeshRenderer> ();
				MeshRenderer meshRenderer = GO_Triangle.GetComponent<MeshRenderer> ();
				meshRenderer.material = matTriangle;
				MeshFilter filter = GO_Triangle.GetComponent<MeshFilter> ();
				Mesh mesh = filter.sharedMesh;
				if (mesh == null) {
					mesh = new Mesh ();
					filter.sharedMesh = mesh;
				}
				Vector2[] uvs = new Vector2[3];
				
				
				int i2 = 0;
				uvs [0] = new Vector2 (pt1.x, pt1.z);
				uvs [1] = new Vector2 (pt2.x, pt2.z);
				uvs [2] = new Vector2 (pt3.x, pt3.z);
				
				Vector3[] normals = new Vector3[3];
				normals [0] = Vector3.up;
				normals [1] = Vector3.up;
				normals [2] = Vector3.up;
				
				
				mesh.vertices = new Vector3[]{pt1,pt2,pt3};
				mesh.uv = uvs;
				mesh.triangles = new int[]{2,0,1};
				mesh.normals = normals;
				
				filter.mesh = mesh;
				mesh.RecalculateNormals ();
				mesh.RecalculateBounds ();
				
				GO_Triangle.transform.SetParent (allChildParent.transform);
			}

		}
	}
	public void CalculateVisibilityForPath()
	{
		//globalPolygon = getObstacleEdges ();

		List<Vector3> endPoints = new List<Vector3> ();
		hTable = new Hashtable ();
		hVisiblePolyTable = new Hashtable ();
		hVisibleTrianglesTable = new Hashtable ();
		hVisibleNewPolygons = new Hashtable ();
		VisibleTriangles.matTriangle = matGreen;
		VisibleTriangles.allChildParent = allLineParent;
		VisibleTriangles.enemyPrefab = enemyPrefab;
		//Extract all end points

		foreach(Line l in mapBG.edges)
		{
			foreach(Vector3 vect in l.vertex)
			{
				if(!endPoints.Contains(vect))
				{
					//finding
					for(int j=0;j<endPoints.Count;j++)
					{
						float dist = (Vector3.Distance(vect,endPoints[j]));
						if(mapDiagonalLength<dist)
						{
							mapDiagonalLength=dist;
						}
					}
					endPoints.Add(vect);
				}
			}
		}
		foreach (Geometry g in globalPolygon) 
		{
			foreach(Line l in g.edges)
			{
				foreach(Vector3 vect in l.vertex)
				{
					if(!endPoints.Contains(vect))
					{
						endPoints.Add(vect);
					}
				}
			}
		}
		/*foreach(Vector3 ptTemp in endPoints)
		{
			showPosOfPoint(ptTemp,Color.blue);
		}*/
		//
		Vector3 normalVect = new Vector3 (0, 1, 0);
		Vector3 xVect = new Vector3 (1, 0, 0);
		//Do for all path points
		int pathIndexTemp = -1;
		foreach(Vector3 pPoint in pathPoints)
		{
			pathIndexTemp++;
			if(bDebugNow && pPoint!=pathPoints[PointToDebug])
			{
				continue;
			}
			if(hVisiblePolyTable.ContainsKey(pPoint))
				continue;
			Vector3 alongX = new Vector3(pPoint.x+2,pPoint.y,pPoint.z);
			List<Geometry> starPoly = new List<Geometry>();
			List<Vector3> arrangedPoints = new List<Vector3> ();
			List<float> angles = new List<float>();

			for(int i=0;i<endPoints.Count;i++)
			{
				Vector3 vect = endPoints[i];
				float sAngle = SignedAngleBetween(pPoint-vect,alongX-pPoint,normalVect);
				/*float sAngleRounded =  Mathf.Round(sAngle * 100f) / 100f;
				//
				bool bDuplicateOnSameObs = false;
				foreach(float angleTemp in angles)
				{
					float angleTempRounded =  Mathf.Round(angleTemp * 100f) / 100f;

					if(angleTempRounded==sAngleRounded)
					{
						//Debug.Log(sAngle);
						int prevIndex = angles.IndexOf(angleTemp);

						if(existOnSameLineOfPolygon(endPoints[i],endPoints[prevIndex]))
						{
							//find which one is greater, then remove/ignore the smaller
							if(sAngle<angleTemp)
							{
								//ignore
								bDuplicateOnSameObs=true;
								endPoints.RemoveAt(i);
								i--;
								break;
							}
							else
							{
								bDuplicateOnSameObs=true;
								angles.RemoveAt(prevIndex);
								endPoints.RemoveAt(prevIndex);
								angles.Add(sAngle);
								i--;
								break;
							}
						}
					}

				}
				if(bDuplicateOnSameObs)
					continue;*/

				angles.Add(sAngle);
			}
			int numTemp = angles.Count;
			while(numTemp>0)
			{
				float minAngle = 370;
				int indexAngle = -1;
				for (int i=0;i<angles.Count;i++)
				{
					if(minAngle>angles[i])
					{
						minAngle = angles[i];
						indexAngle = i;
					}
				}
				//Debug.Log("Angle processed = "+angles[indexAngle]);
				arrangedPoints.Add(endPoints[indexAngle]);
				angles[indexAngle]=370;
				numTemp--;
			}
			//find all intersection points
			List<List<Vector3>> intersectionPointsPerV = new List<List<Vector3>>();
			foreach(Vector3 vect in arrangedPoints)
			{
				Ray rayTemp = new Ray();
				rayTemp.direction = vect - pPoint;
				rayTemp.origin = pPoint;
				Vector3 extendedPoint = rayTemp.GetPoint(mapDiagonalLength);
				//Debug.Log(pPoint+" , "+vect+" , "+extendedPoint);
				Line longRayLine = new Line(pPoint,extendedPoint);
				//showPosOfPoint(vect,Color.red);
				//longRayLine.DrawVector(allLineParent);
				//Find intersection points for longRayLine
				List<Vector3> intersectionPoints = new List<Vector3>();
				//Intersection with holes
				foreach (Geometry g in globalPolygon) 
				{
					foreach(Line l in g.edges)
					{
						if(l.LineIntersectMuntacEndPt(longRayLine)!=0)
						{
							Vector3 intsctPoint = l.GetIntersectionPoint(longRayLine);//LineIntersectionVect(longRayLine);
							//intsctPoint.x
							//if(!intersectionPoints.Contains(intsctPoint))
							if(!ListContainsPoint(intersectionPoints,intsctPoint))
							{
								//Debug.Log("Adding from intersection with holes "+intsctPoint.z);
								intersectionPoints.Add(intsctPoint);
							}
						}
					}
				}
				//Intersection with boundary points
				foreach(Line l in mapBG.edges)
				{
					if(l.LineIntersectMuntacEndPt(longRayLine)!=0)
					{
						Vector3 intsctPoint = l.GetIntersectionPoint(longRayLine);
						//if(!intersectionPoints.Contains(intsctPoint))
						if(!ListContainsPoint(intersectionPoints,intsctPoint))
						{
							//Debug.Log("Adding from intersection with boundary "+intsctPoint.z);
							intersectionPoints.Add(intsctPoint);

						}
					}
				}
				/*foreach(Vector3 intsctPoint in intersectionPoints)
				{
					showPosOfPoint(intsctPoint,Color.red);
					(new Line(pPoint,intsctPoint)).DrawVector(allLineParent);
				}*/
				//Debug.Log(ListContainsPoint(intersectionPoints,vect));
				//Debug.Log(intersectionPoints.Count+"-----------------------------------");

				intersectionPointsPerV.Add(intersectionPoints);
				//Sort Intersection Points
				foreach(List<Vector3> intersectionPts in intersectionPointsPerV)
				{
					List<float> distancesFromV = new List<float>();
					foreach(Vector3 intsctPoint in intersectionPts)
					{
						distancesFromV.Add(Vector3.Distance(pPoint,intsctPoint));
					}
					for(int j=0;j<distancesFromV.Count;j++)
					{
						float leastVal = distancesFromV[j];
						for(int i=j+1;i<distancesFromV.Count;i++)
						{
							if(leastVal>distancesFromV[i])
							{
								leastVal=distancesFromV[i];
							}
						}
						int indexToReplace = distancesFromV.IndexOf(leastVal);
						float tmpA = distancesFromV[indexToReplace];
						distancesFromV[indexToReplace] = distancesFromV[j];
						distancesFromV[j] = tmpA;
						//Interchange values for intersection points
						Vector3 tmpB = intersectionPts[indexToReplace];
						intersectionPts[indexToReplace] = intersectionPts[j];
						intersectionPts[j] = tmpB;
					}
				}

			}
			//Debug.Log(intersectionPointsPerV[0].Count);
			//Remove vertex which is not visible
			//List<int> toRemoveListIndex = new List<int>();


			foreach(List<Vector3> intersectionPts in intersectionPointsPerV)
			{
				int tmpIndex = intersectionPointsPerV.IndexOf(intersectionPts);

				//1st approach
				if(!VectorApprox2(intersectionPts[0],arrangedPoints[tmpIndex]))
				//if((intersectionPts[0]!=arrangedPoints[tmpIndex]))
				{
					//if(arrangedPoints[tmpIndex].x==15f)
					{
						//Debug.Log ("->->->->->->->->->->->->->->->->->->");
						//Debug.Log(intersectionPts[0].x+"!="+arrangedPoints[tmpIndex].x);
						//Debug.Log(intersectionPts[0].z+"!="+arrangedPoints[tmpIndex].z);
						//Debug.Log ("<-<-<-<-<-<-<-<-<-<-<-<-<-<-<-<-<-<-");
						//showPosOfPoint(arrangedPoints[tmpIndex],Color.red);
					}
					intersectionPointsPerV[tmpIndex]=null;
					//Debug.Break();
				}

				//2nd approach
				/*for(int itr1=0;itr1<intersectionPts.Count-1;itr1++)
				{
					Vector3 midPt = Vector3.Lerp(intersectionPts[itr1],intersectionPts[itr1+1],0.5f);
					sdfds;
				}*/
			}
			intersectionPointsPerV.RemoveAll(item=>item==null);

			//Eliminate points
			for(int i=0;i<intersectionPointsPerV.Count;i++)
			{
				List<Vector3> listPts = intersectionPointsPerV[i];
				for(int j=0;j<listPts.Count-1;j++)
				{
					Vector3 mdPt = Vector3.Lerp(listPts[j],listPts[j+1],0.5f);

					/*Line l1 = new Line(pPoint,listPts[j]);
					Line l2 = new Line();
					float? slope1 = l1.getSlope();
					float? slope2 = l2.getSlope();
					if(slope1==null && slope2==null)
					{
						continue;
					}
					if(slope1!=null && slope2!=null)
					{
						if(floatCompare(slope1.Value,slope2.Value))
							continue;
					}*/
					if(existOnSameLineOfPolygon(listPts[j],listPts[j+1]) || existOnSameLineOfPolygon(listPts[j],mdPt))
					{
						continue;
					}
					if(CheckIfInsidePolygon(mdPt) || !mapBG.PointInside(mdPt))
					{
						listPts.RemoveRange(j+1,listPts.Count-j-1);
						break;
					}
				}
			}
			///////////////////////////////////////////////////////Start/////////////////////////////////////////////////////////////////

			//New Elimination:
			Geometry geoVisibleTemp = new Geometry();
			Geometry geoVisibleNew = new Geometry();
			List<VisibleTriangles> listTriangles = new List<VisibleTriangles>();
			int savedSecondIndx=-1;
			for(int i=0;i<=intersectionPointsPerV.Count;i++)
			{

				int currIndx =  i%intersectionPointsPerV.Count;
				int nextIndex = (i+1)%intersectionPointsPerV.Count;
				bool bTriangleAdded = false;

				if(bDebugNow && pPoint==pathPoints[PointToDebug])
				{
					for(int j=0;j<intersectionPointsPerV[currIndx].Count;j++)
					{
						showPosOfPoint(intersectionPointsPerV[currIndx][j],Color.green);

						GameObject allLineParentChild = new GameObject();
						LineRenderer lineR = allLineParentChild.AddComponent<LineRenderer>();
						lineR.material = matGreen;
						lineR.SetWidth(0.02f,0.02f);
						lineR.SetVertexCount(2);
						//lineR.SetPosition(0,l.vertex[0]);
						//lineR.SetPosition(1,l.vertex[1]);
						lineR.SetColors (Color.cyan, Color.cyan);

						Vector3 vertexPt = pPoint;
						lineR.SetPosition (0,vertexPt);
						vertexPt = intersectionPointsPerV[currIndx][j];
						lineR.SetPosition (1, vertexPt);

						allLineParentChild.transform.parent = allLineParent.transform;
						Renderer rend = allLineParentChild.GetComponent<Renderer>();
						rend.material.color = Color.cyan;
						lineR.material.color = Color.cyan;

						/*for(int k=0;k<intersectionPointsPerV[nextIndex].Count;k++)
						{		
							if(existOnSameLineOfPolygon(intersectionPointsPerV[currIndx][j],intersectionPointsPerV[nextIndex][k]))
							{
								GameObject allLineParentChildTemp = new GameObject();
								LineRenderer lineRTemp = allLineParentChildTemp.AddComponent<LineRenderer>();
								lineRTemp.material = matGreen;
								lineRTemp.SetWidth(0.1f,0.1f);
								lineRTemp.SetVertexCount(2);
								lineRTemp.SetColors (Color.cyan, Color.cyan);
								
								Vector3 vertexPtTemp = new Vector3(intersectionPointsPerV[currIndx][j].x,2.0f,intersectionPointsPerV[currIndx][j].z);
								lineRTemp.SetPosition (0,vertexPtTemp);
								vertexPtTemp = new Vector3(intersectionPointsPerV[nextIndex][k].x,2.0f,intersectionPointsPerV[nextIndex][k].z);
								lineRTemp.SetPosition (1, vertexPtTemp);
								
								allLineParentChildTemp.transform.SetParent (allLineParent.transform);
								
								Renderer rendTemp = allLineParentChildTemp.GetComponent<Renderer>();
								rendTemp.material.color = Color.cyan;
								lineRTemp.material.color = Color.cyan;
							}
						}*/
					}
				}
				/*for(int j=0;j<intersectionPointsPerV[i].Count-1;j++)
				{
					geoVisible.edges.Add(new Line(intersectionPointsPerV[i][j],intersectionPointsPerV[i][j+1]));
					
				}*/

				if(intersectionPointsPerV[currIndx].Count==1 && intersectionPointsPerV[nextIndex].Count==1)
				{
					VisibleTriangles vt = new VisibleTriangles();
					vt.pt1 = pPoint;
					vt.pt2 = intersectionPointsPerV[currIndx][0];
					vt.pt3 = intersectionPointsPerV[nextIndex][0];
					listTriangles.Add(vt);
					bTriangleAdded = true;
					geoVisibleTemp.edges.Add(new Line(intersectionPointsPerV[currIndx][0],intersectionPointsPerV[nextIndex][0]));
					geoVisibleNew.edges.Add(new Line(intersectionPointsPerV[currIndx][0],intersectionPointsPerV[nextIndex][0]));
					continue;
				}


				float leastSumF = 10000f;
				float sumIndxF = leastSumF;
				int leastSum = 1000;
				int sumIndx = leastSum;
				int firstIndx=-1;
				int secondIndx=-1;
				for(int j=0;j<intersectionPointsPerV[currIndx].Count;j++)
				{
					for(int k=0;k<intersectionPointsPerV[nextIndex].Count;k++)
					{
						
						if(existOnSameLineOfPolygon(intersectionPointsPerV[currIndx][j],intersectionPointsPerV[nextIndex][k]))
						{
							sumIndxF = Vector3.Distance(pPoint,intersectionPointsPerV[currIndx][j])+Vector3.Distance(pPoint,intersectionPointsPerV[nextIndex][k]);
							if(sumIndxF<leastSumF)
							{
								leastSumF = sumIndxF;
								firstIndx = j;
								secondIndx = k;
							}
							/*Line l1 = new Line(intersectionPointsPerV[currIndx][j],intersectionPointsPerV[nextIndex][k]);
							sumIndx = j+k;
							if(sumIndx<leastSum)
							{
								leastSum = sumIndx;
								firstIndx = j;
								secondIndx = k;
							}*/
						}
					}
				}
				if(firstIndx>=0 && secondIndx>=0)
				{
					VisibleTriangles vt = new VisibleTriangles();
					vt.pt1 = pPoint;
					vt.pt2 = intersectionPointsPerV[currIndx][firstIndx];
					vt.pt3 = intersectionPointsPerV[nextIndex][secondIndx];
					listTriangles.Add(vt);
					bTriangleAdded = true;
					geoVisibleNew.edges.Add(new Line(intersectionPointsPerV[currIndx][firstIndx],intersectionPointsPerV[nextIndex][secondIndx]));
				}
				if(firstIndx>=0 && savedSecondIndx>=0)
				{
					//Debug.Log("firstIndx = "+firstIndx);
					//Debug.Log("savedSecondIndx = "+savedSecondIndx);
					geoVisibleTemp.edges.Add(new Line(intersectionPointsPerV[currIndx][firstIndx],intersectionPointsPerV[currIndx][savedSecondIndx]));
					geoVisibleNew.edges.Add(new Line(intersectionPointsPerV[currIndx][firstIndx],intersectionPointsPerV[currIndx][savedSecondIndx]));
				}
				else
				{
					/*Debug.Log(">>>>>>>>>>>>>>>>>>>>>>>>>>>>");
					Debug.Log("firstIndx = "+firstIndx);
					Debug.Log("secondIndx = "+secondIndx);
					Debug.Log("savedSecondIndx = "+savedSecondIndx);
					Debug.Log("############################");*/
					//savedSecondIndx = secondIndx;
					//continue;
				}
				savedSecondIndx = secondIndx;
				//geoVisibleTemp.edges.Add(new Line(intersectionPointsPerV[i][0],intersectionPointsPerV[i][firstIndx]));
				if(firstIndx>=0 && secondIndx>=0)
				{
					geoVisibleTemp.edges.Add(new Line(intersectionPointsPerV[currIndx][firstIndx],intersectionPointsPerV[nextIndex][secondIndx]));
				}
				if(!bTriangleAdded)
				{
					Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Triangle not added at = "+pathPoints.IndexOf(pPoint));
					if(bDebugNow)
					{
						GameObject allLineParentChildTemp = new GameObject();
						LineRenderer lineRTemp = allLineParentChildTemp.AddComponent<LineRenderer>();
						lineRTemp.material = matGreen;
						lineRTemp.SetWidth(0.08f,0.08f);
						lineRTemp.SetVertexCount(2);
						lineRTemp.SetColors (Color.cyan, Color.cyan);
						Vector3 vertexPtTemp = new Vector3(pPoint.x,2.0f,pPoint.z);
						lineRTemp.SetPosition (0,vertexPtTemp);
						vertexPtTemp = new Vector3(intersectionPointsPerV[currIndx][0].x,2.0f,intersectionPointsPerV[currIndx][0].z);//intersectionPointsPerV[nextIndex][k].z);
						lineRTemp.SetPosition (1, vertexPtTemp);
						allLineParentChildTemp.transform.SetParent (allLineParent.transform);
						Renderer rendTemp = allLineParentChildTemp.GetComponent<Renderer>();
						rendTemp.material.color = Color.cyan;
						lineRTemp.material.color = Color.cyan;


						GameObject allLineParentChildTemp2 = new GameObject();
						LineRenderer lineRTemp2 = allLineParentChildTemp2.AddComponent<LineRenderer>();
						lineRTemp2.material = matGreen;
						lineRTemp2.SetWidth(0.08f,0.08f);
						lineRTemp2.SetVertexCount(2);
						lineRTemp2.SetColors (Color.cyan, Color.cyan);
						Vector3 vertexPtTemp2 = new Vector3(pPoint.x,2.0f,pPoint.z);
						lineRTemp2.SetPosition (0,vertexPtTemp2);
						vertexPtTemp2 = new Vector3(intersectionPointsPerV[nextIndex][0].x,2.0f,intersectionPointsPerV[nextIndex][0].z);//intersectionPointsPerV[nextIndex][k].z);
						lineRTemp2.SetPosition (1, vertexPtTemp);
						allLineParentChildTemp2.transform.SetParent (allLineParent.transform);
						Renderer rendTemp2 = allLineParentChildTemp2.GetComponent<Renderer>();
						rendTemp2.material.color = Color.cyan;
						lineRTemp2.material.color = Color.cyan;

						showPosOfPointRectangle(intersectionPointsPerV[currIndx][0],Color.red);
						showPosOfPointRectangle(intersectionPointsPerV[nextIndex][0],Color.red);
					}
				}
			}
			if(bDebugNow && pPoint==pathPoints[PointToDebug])
			{
				showPosOfPoint(new Vector3(pPoint.x,2.0f,pPoint.z),Color.yellow);
				foreach(VisibleTriangles vt in listTriangles)
				{
					vt.DrawTriangle();
				}
				hVisibleTrianglesTable.Add(pathIndexTemp,listTriangles);
				hVisibleNewPolygons.Add(pathIndexTemp,geoVisibleNew);
				List<Geometry> shadowPoly1 = FindShadowPolygons(pPoint,listTriangles);
				hTable.Add(pPoint,shadowPoly1);

				break;
			}
			hVisibleTrianglesTable.Add(pathIndexTemp,listTriangles);
			hVisibleNewPolygons.Add(pathIndexTemp,geoVisibleNew);
			//Debug.Log("Before remove duplicate = "+geoVisibleTemp.edges.Count);
			geoVisibleTemp.removeDuplicateEdges();

			//geoVisibleTemp.CheckForValidity(pathPoints.IndexOf(pPoint));
			//Debug.Log("After remove duplicate = "+geoVisibleTemp.edges.Count);
			hVisiblePolyTable.Add(pathIndexTemp,geoVisibleTemp);

			List<Geometry> shadowPoly = FindShadowPolygons(pPoint,listTriangles);//FindShadowPolygons(geoVisibleTemp);
			hTable.Add(pathIndexTemp,shadowPoly);
			arrangedPoints.Clear();
			continue;
			/*if(pPoint==pathPoints[9])
			{
				foreach(Line lTemp in geoVisibleTemp.edges)
				{
					lTemp.DrawVector(allLineParent);
				}
				break;
			}*/
			/////////////////////////////////////////////////////Stop///////////////////////////////////////////////////////////////////;
			/*if(pPoint==pathPoints[9])
			{
				for(int i=0;i<intersectionPointsPerV.Count;i++)
				{
					if(intersectionPointsPerV[i].Count<=1)
						continue;
					foreach(Vector3 intersectionPt in intersectionPointsPerV[i])
					{
						//showPosOfPoint(intersectionPt,Color.magenta);

					}
				}
				foreach(List<Vector3> intersectionPts in intersectionPointsPerV)
				{
					foreach(Vector3 intsctPoint in intersectionPts)
					{
						(new Line(pPoint,intsctPoint)).DrawVector(allLineParent);
						break;
					}
				}
				foreach(List<Vector3> intersectionPts in intersectionPointsPerV)
				{
					Line l1 = new Line(intersectionPts[0],intersectionPts[intersectionPts.Count-1]);
					l1.DrawVector(allLineParent);
				}
				break;
			}*/
			//Build geometries
			/*for(int i=0;i<intersectionPointsPerV.Count;i++)
			{
				int nextIndex = (i+1)%intersectionPointsPerV.Count;
				Geometry geoVisible = new Geometry();
				for(int j=0;j<intersectionPointsPerV[i].Count-1;j++)
				{
					geoVisible.edges.Add(new Line(intersectionPointsPerV[i][j],intersectionPointsPerV[i][j+1]));

				}
				if(intersectionPointsPerV[i].Count==1 && intersectionPointsPerV[nextIndex].Count==1)
				{
					geoVisible.edges.Add(new Line(intersectionPointsPerV[i][0],intersectionPointsPerV[nextIndex][0]));
				}
				//All three cases, choose points on same polygon
				else
				{
					bool bOnlyOneEdgeAdded = false;
					for(int j=0;j<intersectionPointsPerV[i].Count;j++)
					{
						if(bOnlyOneEdgeAdded)
							break;
						for(int k=0;k<intersectionPointsPerV[nextIndex].Count;k++)
						{
							if(bOnlyOneEdgeAdded)
								break;

							if(existOnSameLineOfPolygon(intersectionPointsPerV[i][j],intersectionPointsPerV[nextIndex][k]))
							{
								Line l1 = new Line(intersectionPointsPerV[i][j],intersectionPointsPerV[nextIndex][k]);

								geoVisible.edges.Add(l1);
								bOnlyOneEdgeAdded=true;
							}
						}
					}
				}
				starPoly.Add(geoVisible);

			}*/
			//Combining all visible edges
			Geometry visiblePoly = new Geometry();
			foreach(Geometry geo in starPoly)
				visiblePoly.edges.AddRange(geo.edges);
			//visiblePoly = verifyVisibilityPolygon(pPoint,visiblePoly);

			hVisiblePolyTable.Add(pPoint,visiblePoly);

			//List<Geometry> shadowPoly = FindShadowPolygons(visiblePoly);
				//ValidatePolygons(shadowPoly);
				//globalTempArrangedPoints.AddRange(arrangedPoints);
				//globalTempStarPoly = visiblePoly;
				//globalTempShadowPoly = shadowPoly;
				//globalTempintersectionPointsPerV.AddRange(intersectionPointsPerV);
				//bArranged = true;
			//arrangedPoints.Clear();
			//hTable.Add(pPoint,shadowPoly);
		}//End: Do for all path points
	}






	Geometry verifyVisibilityPolygon(Vector3 pPoint,Geometry visiblePoly)
	{
		for(int i=0;i<visiblePoly.edges.Count;i++)
		{
			Vector3 midPt = visiblePoly.edges[i].MidPoint();
			Line l = new Line(pPoint,midPt);
			/*float? slopeL = l.getSlope();
			float? slopeI = visiblePoly.edges[i].getSlope();
			if(slopeI==null && slopeL==null)
			{
				continue;
			}
			if(slopeI!=null && slopeL!=null)
			{
				if(floatCompare(slopeI.Value,slopeL.Value))
					continue;
			}*/
			for(int j=0;j<visiblePoly.edges.Count;j++)
			{
				if(i==j)
					continue;

				if(visiblePoly.edges[j].LineIntersection(l))
				{
					/*float gradx1 = (l.vertex [1].x - l.vertex [0].x);
					float m1=0;
					if(gradx1!=0)
					{
						m1=(l.vertex[1].z - l.vertex[0].z)/gradx1;
					}
					float gradx2 = (visiblePoly.edges[i].vertex [1].x - visiblePoly.edges[i].vertex [0].x);
					float m2=0;
					if(gradx2!=0)
					{
						m2=(visiblePoly.edges[i].vertex[1].z - visiblePoly.edges[i].vertex[0].z)/gradx2;
					}
					//if(gradx1==0 && gradx2==0)
					if(m1==m2)
						continue;*/
					/*if(pPoint==pathPoints[pathIndexToShowShadow])
					{
						l.DrawVector(allLineParent);
						visiblePoly.edges[i].DrawVector(allLineParent);
					}*/
					Vector3 intsctPoint = visiblePoly.edges[j].GetIntersectionPoint(l);
					if(visiblePoly.edges[i].PointOnLine(intsctPoint))
						continue;
					visiblePoly.edges.RemoveAt(i);
					i--;
					break;
				}
			}
		}
		/////////////////////////////
		List<Geometry> visiblePolyList = new List<Geometry>();
		visiblePolyList.Add (visiblePoly);
		int c4 = visiblePolyList.Count;
		visiblePolyList = removeMisformedPolygons(visiblePolyList);
		int c5 = visiblePolyList.Count;
		if(c5!=c4)
			Debug.Log("Visibility Polygon for "+pPoint+" got deleted");
		visiblePoly = visiblePolyList [0];
		/////////////////////////////;
		return visiblePoly;
	}
	/*void ValidatePolygons (List<Geometry> shadowPoly)
	{
		foreach(Geometry g in shadowPoly)
		{
		}
	}
	*/
	private List<Geometry> FindShadowPolygons(Geometry starPoly,int Indx)
	{
		List<Vector3> verticesStar = new List<Vector3> ();
		//foreach(Geometry gStar in starPoly)
		{
			foreach(Line l in starPoly.edges)
			{
				if(!ListContainsPoint(verticesStar,l.vertex[0]))
				{
					verticesStar.Add(l.vertex[0]);
				}
				if(!ListContainsPoint(verticesStar,l.vertex[1]))
				{
					verticesStar.Add(l.vertex[1]);
				}
			}
		}

		List<Geometry> modObstacles = CreateModifiedObstacles(verticesStar);
		Geometry mapModBoundary = CreateModifiedBoundary(verticesStar);
		m_modBoundaryCount=mapModBoundary.edges.Count;
		foreach(Geometry g1 in modObstacles)
		{
			m_modObstcleCount+=g1.edges.Count;
		}



		//Only Debug
		/*bShowShadowEdges = false;
		foreach(Geometry g1 in modObstacles)
		{
			g1.DrawGeometry(allLineParent,matGreen);
			foreach(Line l1 in g1.edges)
			{
				showPosOfPointEnemySized(l1.vertex[0],Color.green);
				showPosOfPointEnemySized(l1.vertex[1],Color.green);
			}
		}
		mapModBoundary.DrawGeometry(allLineParent,matGreen);
		foreach(Line l1 in mapModBoundary.edges)
		{
			showPosOfPointEnemySized(l1.vertex[0],Color.green);
			showPosOfPointEnemySized(l1.vertex[1],Color.green);
		}*/


		//List<VisibleTriangles> listVT = ((List<VisibleTriangles>)(hVisibleTrianglesTable[pathPoints[Indx]]));
		List<VisibleTriangles> listVT = ((List<VisibleTriangles>)(hVisibleTrianglesTable[Indx]));
		//bool res9 = AnotherCheckIfVisible(pathPoints[Indx],pt1,listVT);



		List<Geometry> allGeometries = new List<Geometry> ();
		allGeometries.AddRange (modObstacles);
		allGeometries.Add (mapModBoundary);
		allGeometries.Add (starPoly);
		List<Geometry> shadowPoly = new List<Geometry> ();
		List<Line> listEdges = new List<Line> ();
		foreach(Geometry geo in allGeometries)
		{
			foreach(Line l in geo.edges)
			{
				List<Vector3> pair = l.PointsOnEitherSide(0.002f);
				int ct_visible=0;
				int ct_insideObstacle=0;
				int ct_insideBoundary=0;
				foreach(Vector3 pt in pair)
				{
					if(AnotherCheckIfVisible(pathPoints[Indx],pt,listVT))
					//if(starPoly.PointInside(pt))
					{
						ct_visible++;
					}
					foreach(Geometry g in globalPolygon)
					{
						if(g.PointInside(pt))
						{
							ct_insideObstacle++;
						}
					}
					if(mapBG.PointInside(pt))
					{
						ct_insideBoundary++;
					}
				}
				//if(ct_visible>1)
				{
					//GameObject clone1 = (GameObject)Instantiate(spTemp);
					//clone1.transform.position = pair[0];
					//GameObject clone2 = (GameObject)Instantiate(spTemp);
					//clone2.transform.position = pair[1];
					//Debug.Log("ct_visible="+ct_visible+" &&&&& ct_insideObstacle = "+ct_insideObstacle+"ct_insideBoundary="+ct_insideBoundary);
					//Debug.Log(pair[0].x+","+pair[0].z+" )"+pair[1].x+","+pair[1].z);
				}
				if((ct_visible==0) || (ct_visible==1 && ct_insideObstacle==0 && ct_insideBoundary==2))
				{
					//GameObject clone1 = (GameObject)Instantiate(spTemp);
					//clone1.transform.position = pair[0];
					//GameObject clone2 = (GameObject)Instantiate(spTemp);
					//clone2.transform.position = pair[1];
					listEdges.Add(l);
				}
			}
		}
		globalTempAllShadowLines.AddRange(listEdges);
		//Concatinating all lines into geometries
		//foreach(Line l in listEdges)
		/*for(int i=0;i<listEdges.Count;i++)
		{
			if(listEdges[i]==null)
				continue;
			Geometry shadow = new Geometry();
			shadow.edges.Add(listEdges[i]);
			for(int j=i;j<listEdges.Count;j++)
			{
				if(listEdges[j]==null)
					continue;
				for(int k=0;k<shadow.edges.Count;k++)
				{
					int intsct = listEdges[j].LineIntersectMuntacEndPt(shadow.edges[k]);
					if(intsct!=0)
					{
						shadow.edges.Add(listEdges[j]);
						listEdges[j]=null;
						break;
					}
				}
			}
			shadowPoly.Add(shadow);
		}*/
		///////////////////////////////////
		for(int i=0;i<listEdges.Count;i++)
		{
			if(listEdges[i]==null)
				continue;
			Geometry shadow = new Geometry();
			shadow.edges.Add(listEdges[i]);
			listEdges[i]=null;
			for(int k=0;k<shadow.edges.Count;k++)
			{
				for(int j=0;j<listEdges.Count;j++)
				{
					if(listEdges[j]==null)
						continue;
					//int intsct = listEdges[j].LineIntersectMuntacEndPt(shadow.edges[k]);
					bool intsct = listEdges[j].CommonEndPoint(shadow.edges[k]);
					if(intsct)
					{
						shadow.edges.Add(listEdges[j]);
						listEdges[j]=null;
					}
				}
			}
			shadowPoly.Add(shadow);
		}
		///////////////////////////////////

		//shadowPoly = removeMisformedPolygons(shadowPoly);
		foreach(Geometry g1 in shadowPoly)
		{
			m_ShadowPolygonCount+=g1.edges.Count;
		}
		return shadowPoly;
	}

	private List<Geometry> removeMisformedPolygons(List<Geometry> shadowPoly)
	{
		//Check 1
		for(int i=0;i<shadowPoly.Count;i++)
		{
			if(shadowPoly[i].edges.Count<3)
			{
				shadowPoly.RemoveAt(i);
				i--;
			}

		}
		//Check 2
		for(int i=0;i<shadowPoly.Count;i++)
		{

			for(int j=0;j<shadowPoly[i].edges.Count;j++)
			{
				bool bRemove = false;
				int nTimes1=0;
				int nTimes2=0;
				Line l1 = shadowPoly[i].edges[j];
				for(int k=0;k<shadowPoly[i].edges.Count;k++)
				{
					if(j==k)
						continue;
					Line l2 = shadowPoly[i].edges[k];
					bRemove = l2.PointOnLine(l1.vertex[0]);
					if(bRemove)
						nTimes1++;
					bRemove = l2.PointOnLine(l1.vertex[1]);
					if(bRemove)
						nTimes2++;
				}
				if(nTimes1==0 || nTimes2==0)
				{
					shadowPoly[i].edges.RemoveAt(j);
					j--;
				}
			}
			
		}
		return shadowPoly;
	}

	List<Geometry> CreateModifiedObstacles (List<Vector3> verticesStar)
	{
		List<Geometry> modObstacles = new List<Geometry> ();
		foreach(Geometry g in globalPolygon)
		{
			Geometry obstacle = CreateModifiedPolygon(g,verticesStar);
			modObstacles.Add(obstacle);
		}
		return modObstacles;
	}

	Geometry CreateModifiedBoundary (List<Vector3> verticesStar)
	{
		Geometry mapModBoundary = CreateModifiedPolygon(mapBG,verticesStar);
		return mapModBoundary;
	}
	private Geometry CreateModifiedPolygon(Geometry g,List<Vector3> verticesStar)
	{
		Geometry obstacle = new Geometry();
		//Debug.Log("************Obstacle****************");
		foreach(Line l in g.edges)
		{
			//Debug.Log("************SameLine****************");
			List<Vector3> pointsOnSameline = new List<Vector3>();
			pointsOnSameline.Add(l.vertex[0]);
			foreach(Vector3 vect in verticesStar)
			{
				if(l.PointOnLine(vect))
				{
					if(!ListContainsPoint(pointsOnSameline,vect))
					{
						//Debug.Log(vect.x+","+vect.z);
						pointsOnSameline.Add(vect);
					}
				}
			}
			//Sort points in a line
			for(int i=1;i<pointsOnSameline.Count-1;i++)
			{
				float dist = Vector3.Distance(pointsOnSameline[0],pointsOnSameline[i]);
				int indexToReplace=-1;
				for(int j=i+1;j<pointsOnSameline.Count;j++)
				{
					float dist2 = Vector3.Distance(pointsOnSameline[0],pointsOnSameline[j]);
					if(dist>dist2)
					{
						dist=dist2;
						indexToReplace=j;
					}
				}
				if(indexToReplace>0)
				{
					Vector3 tempVar = pointsOnSameline[i];
					pointsOnSameline[i] = pointsOnSameline[indexToReplace];
					pointsOnSameline[indexToReplace] = tempVar;
				}
			}
			if(!ListContainsPoint(pointsOnSameline,l.vertex[1]))
			{
				pointsOnSameline.Add(l.vertex[1]);
			}
			for(int i=0;i<pointsOnSameline.Count-1;i++)
			{
				//Debug.Log(pointsOnSameline[i].x+","+pointsOnSameline[i].z+" to "+pointsOnSameline[i+1].x+","+pointsOnSameline[i+1].z);
				obstacle.edges.Add(new Line(pointsOnSameline[i],pointsOnSameline[i+1]));
			}
		}
		return obstacle;
	}


	private bool ListContainsPoint(List<Vector3> intersectionPoints,Vector3 intsctPoint)
	{
		float limit = 0.0001f;
		foreach (Vector3 vect in intersectionPoints) 
		{
			if(Vector3.SqrMagnitude(vect-intsctPoint)<limit)
			//if(Mathf.Approximately(vect.magnitude,intsctPoint.magnitude))
				return true;
			//Debug.Log("Points not equal"+vect+" , "+intsctPoint);
		}
		return false;
	}
	private int IndexInList(List<Vector3> myList,Vector3 intsctPoint)
	{
		float limit = 0.0001f;
		int counter = 0;
		foreach (Vector3 vect in myList) 
		{
			if(Vector3.SqrMagnitude(vect-intsctPoint)<limit)
				return counter;
			counter++;
		}
		return -1;
	}
	private bool SignedAngleBetweenNotZero(Vector3 pPoint,Vector3 intsctPt)
	{
		if(CheckIfEndPt(intsctPt))
		{
			return false;
		}
		Vector3 normalVect = new Vector3 (0, 1, 0);
		Vector3 xVect = new Vector3 (1, 0, 0);
		Vector3 alongX = new Vector3(pPoint.x+2,pPoint.y,pPoint.z);
		Line edgeLine = null;

		List<Geometry> allGeometries = new List<Geometry>();
		allGeometries.Add (mapBG);
		allGeometries.AddRange(globalPolygon);

		foreach (Geometry g in allGeometries)
		{
			foreach (Line l in g.edges) 
			{
				if(l.PointOnLine(intsctPt))
				{
					edgeLine = l;
					break;
				}
			}
			if(edgeLine!=null)
				break;
		}
		float sAngle = SignedAngleBetween2(intsctPt-pPoint,edgeLine.vertex[1]-edgeLine.vertex[0],normalVect);
		if(sAngle<=0.1f)
		{
			return false;
		}
		return true;
	}
	private bool CheckIfEndPt(Vector3 pt1)
	{
		foreach (Geometry g in globalPolygon)
		{
			foreach(Line l in g.edges)
			{
				foreach(Vector3 pt in l.vertex)
				{
					if(VectorApprox2(pt,pt1))
					{
						//showPosOfPoint(pt,Color.green);
						return true;
					}
				}
			}
		}
		return false;
	}
	private bool CheckIfInsidePolygon(Vector3 pt)
	{
		bool result = false;
		foreach (Geometry g in globalPolygon)
		{
			result = g.PointInside(pt);
			if(result)
				break;
		}
		return result;
	}
	private bool isTheMidPtOfBoundary(Vector3 pt1)
	{
		foreach (Line l in mapBG.edges) 
		{
			if(VectorApprox(l.MidPoint(),pt1))
			
				return true;

		}
		return false;
	}
	private bool existOnBoundaryOfPolygon(Vector3 pt1)
	{
		bool pt1Found=false;
		foreach (Line l in mapBG.edges) 
		{
			//showPosOfPoint(l.MidPoint(),Color.yellow);
			//pt1Found = l.PointOnLine_LessAccurate(pt1);
			pt1Found = l.PointOnLine(pt1);
			if(pt1Found)
				break;
			//pt1Found = l.POL(pt1);
		}
		return pt1Found;
	}
	public bool PointOnLineB( Line l,Vector3 pt )
	{
		Vector3 a = l.vertex [0];
		Vector3 b = l.vertex [1];
		Vector3 c = pt;
		//Debug.Log ("IsLeft Value: " + ((b.x - a.x) * (pt.z - a.z) - (b.z - a.z) * (c.x - a.x)));
		return floatCompare( ((b.x - a.x)*(pt.z - a.z) - (b.z - a.z)*(c.x - a.x)), 0 );
	}
	private bool existOnSameLineOfPolygon(Vector3 pt1,Vector3 pt2)
	{
		List<Geometry> allGeometries = new List<Geometry>();
		allGeometries.Add (mapBG);
		allGeometries.AddRange(globalPolygon);
		//TODO test this shit
		bool pt1Found=false;
		bool pt2Found=false;
		foreach (Geometry g in allGeometries)
		{
			foreach (Line l in g.edges) 
			{
				//pt1Found = l.PointOnLine(pt1);
				//pt2Found = l.PointOnLine(pt2);
				pt1Found = PointOnLineB(l,pt1);
				pt2Found = PointOnLineB(l,pt2);
				
				if(pt1Found && pt2Found)
				{
					return true;
				}
				/*if(!pt1Found && !pt2Found)
				{
					continue;
				}
				else
				{
					Debug.Log ("Line is " + l.vertex [1] + " to " + l.vertex [0]);
					//Debug.Log("Not on same line"+pt1+" , "+pt2);
					if(pt1Found)
					{
						Debug.Log(pt1+" found");
						Debug.Log(pt2+" NOT found");
					}
					if(pt2Found)
					{
						Debug.Log(pt2+" found");
						Debug.Log(pt1+" NOT found");
					}
					//return false;
				}*/
			}
		}
		return false;
	}
	private bool existOnSamePolygon(Vector3 pt1,Vector3 pt2)
	{
		List<Geometry> allGeometries = new List<Geometry>();
		allGeometries.Add (mapBG);
		allGeometries.AddRange (globalPolygon);
		//TODO test this shit
		//Debug.Log ("In existOnSamePolygon->" + allGeometries.Count + "=mapBG+"+globalPolygon.Count);
		foreach (Geometry g in allGeometries)
		{
			bool pt1Found=false;
			bool pt2Found=false;
			foreach (Line l in g.edges) 
			{
				if(l.PointOnLine(pt1))
				{
					pt1Found = true;
				}
				if(l.PointOnLine(pt2))
				{
					pt2Found = true;
				}
			}
			if(pt1Found && pt2Found)
			{
				return true;
			}
			if(!pt1Found && !pt2Found)
			{
				continue;
			}
			else
			{
				break;
			}
		}
		return false;
	}
	//copied from stackoverflow
	float SignedAngleBetween(Vector3 a, Vector3 b, Vector3 n){
		// angle in [0,180]
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));
		
		// angle in [-179,180]
		float signed_angle = angle * sign;
		
		// angle in [0,360] (not used but included here for completeness)
		float angle360 =  (signed_angle + 180) % 360;
		
		//return signed_angle;
		return angle360;
	}
	float SignedAngleBetween2(Vector3 a, Vector3 b, Vector3 n){
		// angle in [0,180]
		float angle = Vector3.Angle(a,b);
		float sign = Mathf.Sign(Vector3.Dot(n,Vector3.Cross(a,b)));
		
		// angle in [-179,180]
		float signed_angle = angle * sign;
		
		// angle in [0,360] (not used but included here for completeness)
		float angle360 =  (signed_angle + 180) % 360;
		
		//return signed_angle;
		return angle;
	}
	Vector3 [] mapBoundary;
	public List<Geometry> getObstacleEdges()
	{
		//Compute one step of the discritzation
		//Find this is the view
		floor = (GameObject)GameObject.Find ("Floor");
		//LineRenderer line = floor.AddComponent<LineRenderer>();

		
		Vector3 [] vertex = new Vector3[4]; 
		
		//First geometry is the outer one
		List<Geometry> geos = new List<Geometry> ();
		
		
		//Drawing lines
		//VectorLine.SetCamera3D(Camera.current); 
		
		//Floor
		Vector3[] f = new Vector3[4];
		MeshFilter mesh = (MeshFilter)(floor.GetComponent<MeshFilter>());
		Vector3[] t = mesh.sharedMesh.vertices; 
		
		Geometry tempGeometry = new Geometry (); 
		
		vertex [0] = mesh.transform.TransformPoint (t [1]);
		vertex [1] = mesh.transform.TransformPoint (t [0]);
		vertex [2] = mesh.transform.TransformPoint (t [23]);
		vertex [3] = mesh.transform.TransformPoint (t [11]);

		
		vertex [0].y = 1; 
		vertex [1].y = 1; 
		vertex [2].y = 1; 
		vertex [3].y = 1; 


		//these were in tempGeometry previously
		
		//Disabled Temporarily - Find a way to avoid floor when checking for obstacle collision
		//geos.Add (tempGeometry);
		
		mapBoundary = new Vector3[4]; //the map's four corners
		
		for (int i = 0; i < 4; i++) {
			mapBoundary [i] = vertex [i];
		}
		if(currSceneName=="Crash.unity")
		{
			obsGeos.Clear();
			drawingFromFile_LevelCrash();
			return obsGeos;
		}
		/*if(currSceneName=="myCrash.unity")
		{
			mapBG = getBoundaryLevelMyCrash();
			obsGeos = getObstaclesMyCrash();
			return obsGeos;
		}*/
		
		//Geometry mapBG = new Geometry (); 
		for (int i = 0; i < 4; i++)
			mapBG.edges.Add( new Line( mapBoundary[i], mapBoundary[(i + 1) % 4]) );
		//Debug.Log ("mapBg" + mapBG.edges.Count);
		//mapBG.DrawVertex (GameObject.Find ("temp"));
		//mapBG.DrawGeometry(GameObject.find);
		
		GameObject[] obs = GameObject.FindGameObjectsWithTag ("Obs");
		if(obs == null)
		{
			//Debug.Log("Add tag geos to the geometries"); 
			return null; 
		}
		//data holder
		//Triangulation triangulation = GameObject.Find ("Triangulation").GetComponent<Triangulation> (); 
		//triangulation.points.Clear ();
		//triangulation.colours.Clear (); 
		
		//Only one geometry for now

		foreach (GameObject o in obs) 
		{
			mesh = o.GetComponent<MeshFilter>();
			t = mesh.sharedMesh.vertices; 
			tempGeometry = new Geometry();
			
			vertex [0] = mesh.transform.TransformPoint (t [6]);
			vertex [1] = mesh.transform.TransformPoint (t [8]);
			vertex [3] = mesh.transform.TransformPoint (t [7]);
			vertex [2] = mesh.transform.TransformPoint (t [9]);
			
			vertex [0].y = 1;
			vertex [2].y = 1;
			vertex [1].y = 1;
			vertex [3].y = 1;
			for (int i = 0; i< vertex.Length; i+=1) 
			{
				if (i < vertex.Length - 1)
					tempGeometry.edges.Add (new Line (vertex [i], vertex [i + 1]));
				else 	       
					tempGeometry.edges.Add (new Line (vertex [0], vertex [i]));
			}	
			geos.Add (tempGeometry); 
		}
		
		//lines are defined by all the points in  obs
		//lines = new List<Line> ();
		
		obsGeos.Clear ();
		foreach (Geometry g in geos) {
			obsGeos.Add(g);
		}
		
		
		//Create empty GameObject
		GameObject temp = GameObject.Find("temp");
		DestroyImmediate(temp);
		temp = new GameObject("temp");
		//CODESPACE
		//Merging Polygons
		/*for (int i = 0; i < obsGeos.Count; i++) {
			for (int j = i + 1; j < obsGeos.Count; j++) {
				//check all line intersections
				if( obsGeos[i].GeometryIntersect( obsGeos[j] ) ){
					//Debug.Log("Geometries Intersect: " + i + " " + j);
					Geometry tmpG = obsGeos[i].GeometryMerge( obsGeos[j] ); 
					//remove item at position i, decrement i since it will be increment in the next step, break
					obsGeos.RemoveAt(j);
					obsGeos.RemoveAt(i);
					obsGeos.Add(tmpG);
					i--;
					break;
				}
			}
		}*/
		for (int i = 0; i < obsGeos.Count; i++){
			for (int j = i + 1; j < obsGeos.Count; j++) {
				//Check to see if two geometries intersect
				if( obsGeos[i].GeometryIntersect( obsGeos[j] ) ){
					Geometry tmpG = obsGeos[i].GeometryMerge( obsGeos[j], 0 );
					//remove item at position i, decrement i since it will be incremented in the next step, break
					obsGeos.RemoveAt(j);
					obsGeos.RemoveAt(i);
					obsGeos.Add(tmpG);
					i--;
					break;
				}
			}
		}
		//		mapBG.DrawGeometry (temp);
		
		/*List<Geometry> finalPoly = new List<Geometry> ();//Contains all polygons that are fully insde the map
		foreach ( Geometry g in obsGeos ) {
			if( mapBG.GeometryIntersect( g ) && !mapBG.GeometryInside( g ) ){
				mapBG = mapBG.GeometryMergeInner( g );
				mapBG.BoundGeometry( mapBoundary );
			}
			else
				finalPoly.Add(g);
		}*/

			//Check for obstacles that intersect the map boundary
			//and change the map boundary to exclude them
		List<Geometry> finalPoly = new List<Geometry> ();//Contains all polygons that are fully insde the map
			finalPoly = new List<Geometry> ();//Contains all polygons that are fully insde the map
			int xid = 0;
			foreach (Geometry g in obsGeos) {
				//	g.DrawGeometry(GameObject.Find(	"temp" ), xid++ );		
			}
			xid = -1;
			foreach ( Geometry g in obsGeos ) {
				if( mapBG.GeometryIntersect( g ) && !mapBG.GeometryInsideMap( g ) ){
					Geometry tempBG = new Geometry();
					tempBG = mapBG.GeometryMergeInner( g, xid );
					//if( inkscape )
						tempBG.BoundGeometry( mapBG );
					/*else
						tempBG.BoundGeometryCrude( mapBoundary );
						*/
					mapBG = tempBG;
				}
				else
					finalPoly.Add(g);
			}
			//TODO:Check if mapBG has any disjoint parts
			//mapBG.getSortedEdges ();
			/*int cnt = 0;
			foreach (Geometry g in finalPoly) {
				foreach( Line l in g.edges ){
					l.name = "Wall " + cnt++.ToString();
					totalGeo.edges.Add( l );
				}
			}
			foreach( Line l in mapBG.edges ){
				mapBGPerimeter += l.Magnitude();
				totalGeo.edges.Add( l );
			}
			totalGeoVerts = totalGeo.GetVertex ();*/
		
		return finalPoly;
	}
}
