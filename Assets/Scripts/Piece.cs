using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Piece : MonoBehaviour
{
	private Animator anim;
	private BoardManager manager;
	private SpriteRenderer sprite;
	public bool alreadyPlaced = false;
	public BoardManager.color state;
    void Start()
    {
		sprite = this.GetComponent<SpriteRenderer>();
		manager = GameObject.Find("Board").GetComponent<BoardManager>();
		anim = gameObject.GetComponent<Animator>();	
		if(!alreadyPlaced){
			state = BoardManager.color.off;
			sprite.enabled = false;
		}
		else{
			sprite.enabled = true;
			drawPiece();
		}
	}
	public void fliptowhite(){
		state = BoardManager.color.white;
		anim.SetTrigger("fliptowhite");
	}
	public void fliptoblack(){
		state = BoardManager.color.black;
		anim.SetTrigger("fliptoblack");
	}
	
	void white(){
		state = BoardManager.color.white;
		drawPiece();
	}
	void black(){
		state = BoardManager.color.black;
		drawPiece();
	}
	public void drawPiece(){
		if(state == BoardManager.color.white){
			state = BoardManager.color.white;
			anim.SetTrigger("white");
		}
		if(state == BoardManager.color.black){ 
			state = BoardManager.color.black;
			anim.SetTrigger("black");
		}
	}
	
	
	
	void OnMouseDown(){
		if(!manager.playing){ return; }
		string objectName = (string)gameObject.name;
		int x;
		int y;
		int.TryParse (Char.ToString(objectName[0]),out x);
		int.TryParse (Char.ToString(objectName[1]), out y);
		if(!manager.isValidMove(manager.state, x, y, transform.position)){ return;}		
		if(sprite.enabled == false){
			sprite.enabled = true;
			alreadyPlaced = true;
		}
		else{
			return;
		}
		if(manager.state == BoardManager.color.white){ 
			white();	
		}
		if(manager.state == BoardManager.color.black){ 
			black();
		}
	}
	void PlayFlip(){
		manager.playSound();
	}
}
