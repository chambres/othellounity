using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 using UnityEngine.UI;
using System;
 using UnityEngine.SceneManagement;

public class BoardManager : MonoBehaviour
{
	
	public GameObject mousePiece;
	
	
	

	public enum color{
		black=0,
		white=1,
		off=-1
	}
	
	public color state;
	
	public bool playing = true;
	
	[HideInInspector]
	public List<GameObject> a = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> b = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> c = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> d = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> e = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> f = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> g = new List<GameObject>();
	[HideInInspector]
	public List<GameObject> h = new List<GameObject>();
	
	
	public AudioSource flipSound;
	
	
	public List<List<GameObject>> board = new List<List<GameObject>>();
	
	public GameObject wintext;
	
	
	public bool validPosition(int x, int y){
		return (x >= 0 && y >= 0 && x < 8 && y < 8);
	}
		
	public color getOppositeColor(color a){
		if(a == color.white){	
			return color.black;	
		}
		else{ 
			return color.white;		
		}
	}
	
	int[] sOFFSET_MOVE_ROW = {-1, -1, -1,  0,  0,  1,  1,  1};
	int[] sOFFSET_MOVE_COL = {-1,  0,  1, -1,  1, -1,  0,  1};
		
	List<int> affectedMovesX = new List<int>();
	List<int> affectedMovesY = new List<int>();
	
	public bool isValidMove(color c, int row, int col) {
		
		// check whether this square is empty
		if (board[row][col].gameObject.GetComponent<Piece>().state != color.off){
		
			return false;
		}
		
		//char oppPiece = (piece == sBLACK_PIECE) ? sWHITE_PIECE : sBLACK_PIECE;
		color oppPiece = getOppositeColor(c);
		
		
		
		bool isValid = false;
		// check 8 directions
		
		
		for (int i = 0; i < 8; ++i) {
			int curRow = row + sOFFSET_MOVE_ROW[i];
			int curCol = col + sOFFSET_MOVE_COL[i];
			
			
			bool hasOppPieceBetween = false;

			while (curRow >=0 && curRow < 8 && curCol >= 0 && curCol < 8) {
				
				//Debug.Log("this vs that: " + (board[curRow][curCol].gameObject.GetComponent<Piece>().state == oppPiece));
				if (board[curRow][curCol].gameObject.GetComponent<Piece>().state == oppPiece)
					hasOppPieceBetween = true;
				else if ((board[curRow][curCol].gameObject.GetComponent<Piece>().state == c) && hasOppPieceBetween)
				{
					isValid = true;
			
					break;
				}
				
				
				else{
					break;
				}
				

				curRow += sOFFSET_MOVE_ROW[i];
				curCol += sOFFSET_MOVE_COL[i];
				
				
				

				
			}
			if (isValid)//break;
				continue;
		}	
		return isValid;
	}	
	public bool isValidMove(color c, int row, int col, Vector3 loc) {
		affectedMovesX.Clear();
		affectedMovesY.Clear();
		// check whether this square is empty
		if (board[row][col].gameObject.GetComponent<Piece>().state != color.off){
			return false;
		}

		//char oppPiece = (piece == sBLACK_PIECE) ? sWHITE_PIECE : sBLACK_PIECE;
		color oppPiece = getOppositeColor(c);
		bool isValid = false;
		int count=0;
		for (int i = 0; i < 8; ++i) {
			List<int> tmpX = new List<int>();
			List<int> tmpY = new List<int>();
			int curRow = row + sOFFSET_MOVE_ROW[i];
			int curCol = col + sOFFSET_MOVE_COL[i];
			bool hasOppPieceBetween = false;
			while (curRow >=0 && curRow < 8 && curCol >= 0 && curCol < 8) {
				//Debug.Log("this vs that: " + (board[curRow][curCol].gameObject.GetComponent<Piece>().state == oppPiece));
				if (board[curRow][curCol].gameObject.GetComponent<Piece>().state == oppPiece)
					hasOppPieceBetween = true;
				else if ((board[curRow][curCol].gameObject.GetComponent<Piece>().state == c) && hasOppPieceBetween)
				{
					isValid = true;
					for(int n = 0; n < tmpX.Count; n++){
						affectedMovesX.Add(tmpX[n]);
						affectedMovesY.Add(tmpY[n]);
					}
					count++;
					break;
				}
				else{
					break;
				}
				tmpX.Add(curRow);
				tmpY.Add(curCol);
				curRow += sOFFSET_MOVE_ROW[i];
				curCol += sOFFSET_MOVE_COL[i];
			}
			if (isValid)//break;
				continue;
		}
		if(isValid)
			StartCoroutine(flip(loc, row, col));
		return isValid;
	}
	public void playSound(){
		flipSound.Play();
	}

	bool blackValid;
	bool whiteValid;

	public bool validMovesLeft(){
		bool blackV = false;
		bool whiteV = false;
		for (int i = 0; i < board.Count; i++)
		{
			for (int j = 0; j < board.Count; j++)
			{
				if (isValidMove(color.black, i, j) || isValidMove(color.white, i, j))
				{
					return false;
				}
			}
		}

		return true;
	}

	public bool validBlack()
	{
		for (int i = 0; i < board.Count; i++)
		{
			for (int j = 0; j < board.Count; j++)
			{
				if (isValidMove(color.black, i, j))
				{
					return true;
				}
			}
		}

		return false;
	}

	public bool validWhite()
	{
		for (int i = 0; i < board.Count; i++)
		{
			for (int j = 0; j < board.Count; j++)
			{
				if (isValidMove(color.white, i, j))
				{
					return true ;
				}
			}
		}

		return false;
	}

	public string winner(){
		int b=0;
		int w=0;
		for(int i = 0; i < board.Count; i++){
			for(int j = 0; j < board.Count; j++){
				if(board[i][j].GetComponent<Piece>().state == color.white)
					w++;
				if(board[i][j].GetComponent<Piece>().state == color.black)
					b++;
					
			}
		}
		if(b>w)
			return "Black";
		if(w>b)
			return "White";
		if(w==b)
			return "Both players";
		return "";
	}
	float r=0f;
	Vector3 aoom;

	private List<GameObject> SomeMethod(float r, Vector3 loc, int row, int col){
		//Collider2D[] a =  Physics2D.OverlapBoxAll(loc, new Vector2(r,r), 0f);
		Collider2D[] a = Physics2D.OverlapBoxAll(loc, new Vector2(r, r), 0f);
		Debug.Log(r);
		aoom = loc;
		List<GameObject> final = new List<GameObject>();
		int o;
		int p;
		

		
		for(int i = 0; i < a.Length; i++){
			int.TryParse(Char.ToString(a[i].gameObject.name[0]), out o);
			int.TryParse(Char.ToString(a[i].gameObject.name[1]), out p);
				if(!(loc == a[i].gameObject.transform.position)){
					final.Add(a[i].gameObject);
				};
			
				
			
		}
		return final;
	}
	
	private void OnDrawGizmos()
    {	
        Gizmos.color = Color.red;
        // Gizmos.matrix = Matrix4x4.TRS(this.transform.position, this.transform.rotation, new Vector3(1,1,1));
        // Gizmos.DrawWireCube(this.transform.position, new Vector2(radius, radius));
		Gizmos.DrawWireCube(aoom, new Vector3(r,r, 1));
    }

	IEnumerator flip(Vector3 loc, int row, int col){


		//changeMousePiece();
		//Debug.Log(String.Join(",", affectedMovesX));

		for (r = 2.5f; r < 18f; r += 2f)
		{
			List<GameObject> surrounding = SomeMethod(r, loc, row, col);
			for (int obj = 0; obj < surrounding.Count; obj++)
			{

				int x;
				int y;
				int.TryParse(Char.ToString(surrounding[obj].name[0]), out x);
				int.TryParse(Char.ToString(surrounding[obj].name[1]), out y);

					

				//Debug.Log(x+" "+y);

				for (int i = 0; i < affectedMovesX.Count; i++)
				{
					int n = affectedMovesX[i];
					int m = affectedMovesY[i];
					//Debug.Log(n+" "+m);

					Debug.Log(affectedMovesX.Count);
					if (n == x && y == m)
					{
						
						//board[x][y].gameObject.GetComponent<Piece>().();
						if (state == color.white)
						{
							board[n][m].gameObject.GetComponent<Piece>().fliptowhite();
							affectedMovesX.RemoveAt(i);
							affectedMovesY.RemoveAt(i);
						}
						if (state == color.black)
						{
							board[n][m].gameObject.GetComponent<Piece>().fliptoblack();
							affectedMovesX.RemoveAt(i);
							affectedMovesY.RemoveAt(i);
						}

						if (affectedMovesX.Count == 0)
						{
							Debug.Log("done!"); 
							changeState();

							if (validBlack() && !validWhite() && state == color.white)
							{
								Debug.Log("nowhite");
								changeState();
								mousePiece.GetComponent<Animator>().SetTrigger("black");

							}
							else if (!validBlack() && validWhite() && state == color.black)
							{
								
								Debug.Log("noblack");
								changeState();
								mousePiece.GetComponent<Animator>().SetTrigger("white");

							}
							else if (validMovesLeft())
							{
								mousePiece.SetActive(false);
								Debug.Log("win");
								playing = false;
								wintext.SetActive(true);
								wintext.GetComponent<Text>().text = winner() + " win!";
							}
							else
							{
								changeState();
								Debug.Log(state);
								changeMousePiece();
								changeState();
							}
						
							
							
                            
						}

					}
					//affectedMovesX.RemoveAt(i);
					//affectedMovesY.RemoveAt(i);
				}

			}

			

			yield return new WaitForSeconds(.2f);
		}


		yield return new WaitForSeconds(0f);
			// for(int i = 0; i < affectedMovesX.Count; i++){
				// int x = affectedMovesX[i];
				// int y = affectedMovesY[i];
				
				
				
				// //board[x][y].gameObject.GetComponent<Piece>().();
				// if(state == color.white)
					// board[x][y].gameObject.GetComponent<Piece>().fliptowhite();
				// if(state == color.black)
					// board[x][y].gameObject.GetComponent<Piece>().fliptoblack();
			// }	
	}

	
	
	public OrderCollider o;
	
    // Start is called before the first frame update
    void Start()
    {
		wintext.SetActive(false);
		
		state = color.black;
        board.Add(a); 		board.Add(b); 		board.Add(c); 		board.Add(d); 		board.Add(e); 		board.Add(f); 		board.Add(g); 		board.Add(h);
		
		for(int i = 0; i < board.Count; i++){
			for(int j = 0; j < board.Count; j++){
				board[i][j].gameObject.name = ""+i+j;
			}
		}
		
		

		
		
	}
	
	public void changeState(){

		if(state==color.black){
		
			state = color.white;
			
		}
		else if(state==color.white){
			
			state = color.black;
		}
		
			
	}
	public void changeMousePiece(){

		if(state==color.black){
			mousePiece.GetComponent<Animator>().SetTrigger("white");
		}
		else if(state==color.white){
			mousePiece.GetComponent<Animator>().SetTrigger("black");
		}
		
			
	}
	
	
	
	
	
    // Update is called once per frame
    void Update()
    {
		Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePosition.z = Camera.main.transform.position.z + Camera.main.nearClipPlane;
        mousePiece.transform.position = mousePosition;

		if(Input.GetKeyDown(KeyCode.R))
			 SceneManager.LoadScene( SceneManager.GetActiveScene().name );
	

    }
	
	
}

