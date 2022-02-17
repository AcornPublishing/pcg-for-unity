using UnityEngine;
using System.Collections;
using UnityEngine.UI;	//Allows us to use UI.

//Player inherits from MovingObject, our base class for objects that can move, Enemy also inherits from this.
public class Player : MovingObject
{
	public int wallDamage = 1;					//How much damage a player does to a wall when chopping it.
	public Text healthText;						//UI Text to display current player health total.
	private Animator animator;					//Used to store a reference to the Player's animator component.
	private int health;							//Used to store player health points total during level.
	
	
	//Start overrides the Start function of MovingObject
	protected override void Start ()
	{
		//Get a component reference to the Player's animator component
		animator = GetComponent<Animator>();
		
		//Get the current health point total stored in GameManager.instance between levels.
		health = GameManager.instance.healthPoints;
		
		//Set the healthText to reflect the current player health total.
		healthText.text = "Health: " + health;
		
		//Call the Start function of the MovingObject base class.
		base.Start ();
	}
	
	private void Update ()
	{
		//If it's not the player's turn, exit the function.
		if(!GameManager.instance.playersTurn) return;
		
		int horizontal = 0;  	//Used to store the horizontal move direction.
		int vertical = 0;		//Used to store the vertical move direction.
		
		//Get input from the input manager, round it to an integer and store in horizontal to set x axis move direction
		horizontal = (int) (Input.GetAxisRaw ("Horizontal"));
		
		//Get input from the input manager, round it to an integer and store in vertical to set y axis move direction
		vertical = (int) (Input.GetAxisRaw ("Vertical"));
		
		//Check if moving horizontally, if so set vertical to zero.
		if(horizontal != 0)
		{
			vertical = 0;
		}

		//Check if we have a non-zero value for horizontal or vertical
		if(horizontal != 0 || vertical != 0)
		{
			//Call AttemptMove passing in the generic parameter Wall, since that is what Player may interact with if they encounter one (by attacking it)
			//Pass in horizontal and vertical as parameters to specify the direction to move Player in.
			AttemptMove<Wall> (horizontal, vertical);
		}
	}
	
	//AttemptMove overrides the AttemptMove function in the base class MovingObject
	//AttemptMove takes a generic parameter T which for Player will be of the type Wall, it also takes integers for x and y direction to move in.
	protected override bool AttemptMove <T> (int xDir, int yDir)
	{	
		//Call the AttemptMove method of the base class, passing in the component T (in this case Wall) and x and y direction to move.
		bool hit = base.AttemptMove <T> (xDir, yDir);
		
		//Set the playersTurn boolean of GameManager to false now that players turn is over.
		GameManager.instance.playersTurn = false;

		return hit;
	}
	
	
	//OnCantMove overrides the abstract function OnCantMove in MovingObject.
	//It takes a generic parameter T which in the case of Player is a Wall which the player can attack and destroy.
	protected override void OnCantMove <T> (T component)
	{
		//Set hitWall to equal the component passed in as a parameter.
		Wall hitWall = component as Wall;
		
		//Call the DamageWall function of the Wall we are hitting.
		hitWall.DamageWall (wallDamage);
		
		//Set the attack trigger of the player's animation controller in order to play the player's attack animation.
		animator.SetTrigger ("playerChop");
	}
	
	//LoseHealth is called when an enemy attacks the player.
	//It takes a parameter loss which specifies how many points to lose.
	public void LoseHealth (int loss)
	{
		//Set the trigger for the player animator to transition to the playerHit animation.
		animator.SetTrigger ("playerHit");
		
		//Subtract lost health points from the players total.
		health -= loss;
		
		//Update the health display with the new total.
		healthText.text = "-"+ loss + " Health: " + health;
		
		//Check to see if game has ended.
		CheckIfGameOver ();
	}
	
	
	//CheckIfGameOver checks if the player is out of health points and if so, ends the game.
	private void CheckIfGameOver ()
	{
		//Check if health point total is less than or equal to zero.
		if (health <= 0) 
		{	
			//Call the GameOver function of GameManager.
			GameManager.instance.GameOver ();
		}
	}
}

