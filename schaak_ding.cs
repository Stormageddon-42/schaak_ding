using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI : MonoBehaviour {

	public bool chosen_side;

	private bool white = true;

	private int[][] board = new int[8][];
	List<int[][]> possible_positions = new List<int[][]>();

	private bool can_choose = true;
	private int chosen_x;
	private int chosen_y;
	private GameObject choose_sprite;

	public GameObject white_rook;
	public GameObject white_knight;
	public GameObject white_bishop;
	public GameObject white_pawn;
	public GameObject white_queen;
	public GameObject white_king;

	public GameObject black_rook;
	public GameObject black_pawn;
	public GameObject black_knight;
	public GameObject black_bishop;
	public GameObject black_queen;
	public GameObject black_king;

	public GameObject chosen_sprite;


	//-----------------------start--------------------------------
	void Start () {

		//create board

		for (int i = 0; i < 8; i++) {
			board [i] = new int[8];
		}
		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {
				board [i][j] = 0;
			}
		}
			
		set_board ();
		create_pos_move(white);

	}
	//----------------------------------------------------------------
	/* 0  empty
		 * 1  white pawn
		 * 2  black pawn
		 * 3  white knight
		 * 4  black knight
		 * 5  white bishop
		 * 6  black bishop
		 * 7  white rook
		 * 8  black rook
		 * 9  white queen
		 * 10  black queen
		 * 11  white king
		 * 12  black king
		 */
	//-----------------------------------------------------------------
	void Update() {


		if (white) {
			//----------------------------------------------------------------niet aanzitten-------------------------------------------------------------------------
			if (Input.GetKeyDown (KeyCode.Mouse0)) {
				Vector2 pos = Camera.main.ScreenToWorldPoint (Input.mousePosition);

				if (!(Mathf.RoundToInt (pos.x) > 7)) {
					if (!(Mathf.RoundToInt (pos.x) < 0)) {
						if (!(Mathf.RoundToInt (pos.y) > 7)) {
							if (!(Mathf.RoundToInt (pos.y) < 0)) {

								if (can_choose) {

									if (!(board [Mathf.RoundToInt (pos.y)] [Mathf.RoundToInt (pos.x)] == 0)) {

										choose_sprite = Instantiate (chosen_sprite, new Vector3 (Mathf.RoundToInt (pos.x), Mathf.RoundToInt (pos.y), 1), Quaternion.identity);
										can_choose = false;

										chosen_x = Mathf.RoundToInt (pos.x);
										chosen_y = Mathf.RoundToInt (pos.y);

									}

								} else {

									if ((Mathf.RoundToInt (pos.x) == chosen_x) && (Mathf.RoundToInt (pos.y) == chosen_y)) {

										Destroy (choose_sprite);
										can_choose = true;

									} else {


										int[][] new_board = new int[8][];
										CopyBoard (new_board, board);

										new_board [Mathf.RoundToInt (pos.y)] [Mathf.RoundToInt (pos.x)] = new_board [chosen_y] [chosen_x];
										new_board [chosen_y] [chosen_x] = 0;


										int too_full = 0;

										foreach (int[][] x in possible_positions) {


											//----for debugging-------
											too_full++;

											if (too_full > 500) {
												break;
											}
											// -----------------------

											int checks = 0;

											for (int k = 0; k < 8; k++) {
												for (int l = 0; l < 8; l++) {

													if (x [k] [l] == new_board [k] [l]) {
														checks++;
													}
												}
											}

											if (checks == 64) {
											
												board = (int[][])new_board.Clone ();
												draw_board ();
												Destroy (choose_sprite);
												can_choose = true;
												possible_positions.Clear ();
												white = !white;
												create_pos_move (white);
										
												break;

											} else {
												checks = 0;
											}
										}
									}
								}
							}
						}
					}
				}			
			}
		} else {

			int[][] test_board = new int[8][];
			CopyBoard (test_board, board);

			foreach (int[][] x in possible_positions) {

				if (board_score(x, false) <= board_score(test_board, false)) {
				
					CopyBoard (test_board, x);
				}
			}

			board = possible_positions[Random.Range(0, possible_positions.Count)];
			draw_board ();
			Destroy (choose_sprite);
			can_choose = true;
			possible_positions.Clear ();
			white = !white;
			create_pos_move (white);
		}
	}

	//--------------------------------------------------------------------------niet aanzitten-------------------------------------------------------------


	void create_pos_move(bool white) {

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {


				if (white) {
					//pawns-----------------------------------------
					if (board [i] [j] == 1) {
					
						if (i == 1) {

							int[][] p1 = new int[8][];
							int[][] p2 = new int[8][];
							CopyBoard (p1, board);
							CopyBoard (p2, board);

							//two steps
							if ((p1 [3] [j] == 0) && ((p1 [2] [j] == 0))) {
								p1 [3] [j] = 1;
								p1 [1] [j] = 0;
								possible_positions.Add (p1);
							}
							//one step
							if (p2 [2] [j] == 0) {
								p2 [2] [j] = 1;
								p2 [1] [j] = 0;
								possible_positions.Add (p2);
							} 
						}

						if (i > 0 && i < 7) {

							int[][] p1 = new int[8][];
							int[][] p2 = new int[8][];
							int[][] p3 = new int[8][];

							CopyBoard (p1, board);
							CopyBoard (p2, board);
							CopyBoard (p3, board);


							//one step
					
							if (p1 [i + 1] [j] == 0) {
								p1 [i + 1] [j] = 1;
								p1 [i] [j] = 0;
								possible_positions.Add (p1);
							}

							if (j < 7) {

								//-------------- capture
								if ((p2 [i + 1] [j + 1] % 2 == 0) && (!(p2 [i + 1] [j + 1] == 0))) {
									p2 [i + 1] [j + 1] = 1;
									p2 [i] [j] = 0;
									possible_positions.Add (p2);

								} 
							}

							if (j > 0) {

								if ((p2 [i + 1] [j - 1] % 2 == 0) && (!(p2 [i + 1] [j - 1] == 0))) {
									p3 [i + 1] [j - 1] = 1;
									p3 [i] [j] = 0;
									possible_positions.Add (p3);

								} 
							}
						}
					}
				} else {
					if (board [i] [j] == 2) {
						
						if (i == 6) {

							int[][] p1 = new int[8][];
							int[][] p2 = new int[8][];
							CopyBoard (p1, board);
							CopyBoard (p2, board);

							//two steps
							if ((p1 [4] [j] == 0) && ((p1 [5] [j] == 0))) {
								p1 [4] [j] = 2;
								p1 [6] [j] = 0;
								possible_positions.Add (p1);
							}
							//one step
							if (p2 [5] [j] == 0) {
								p2 [5] [j] = 2;
								p2 [6] [j] = 0;
								possible_positions.Add (p2);
							}
						}

						if (i > 0 && i < 7) {

							int[][] p1 = new int[8][];
							int[][] p2 = new int[8][];
							int[][] p3 = new int[8][];

							CopyBoard (p1, board);
							CopyBoard (p2, board);
							CopyBoard (p3, board);


							//one step
							if (p1 [i - 1] [j] == 0) {
								p1 [i - 1] [j] = 2;
								p1 [i] [j] = 0;
								possible_positions.Add (p1);
							}

							if (j < 7) {

								//-------------- capture
								if ((p2 [i - 1] [j + 1] % 2 == 1)) {
									p2 [i - 1] [j + 1] = 2;
									p2 [i] [j] = 0;
									possible_positions.Add (p2);
								}
							}

							if (j > 0) {

								if ((p3 [i - 1] [j - 1] % 2 == 1)) {
									p3 [i - 1] [j - 1] = 2;
									p3 [i] [j] = 0;
									possible_positions.Add (p3);
								}
							}
						}
					}
				}
  					
				if (white) {

					if (board [i] [j] == 3) {

						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						Knight_move (i, j, p1, 3);
					}

					if (board [i] [j] == 5) {

						int[][] p1 = new int[8][];
						CopyBoard (p1, board);
		
						Bishop_move (i, j, p1, 5, 0);
					}

					if (board [i] [j] == 7) {

						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						Rook_move (i, j, p1, 7, 0);
					}

					if (board [i] [j] == 9) {
						
						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						Queen_move (i, j, p1, 9, 0); 
					}

					if (board [i] [j] == 11) {

						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						King_move (i, j, board, 11);
					}
				} else {
					if (board [i] [j] == 4) {

						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						Knight_move (i, j, p1, 4);
					}

					if (board [i] [j] == 6) {
						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						Bishop_move (i, j, p1, 6, 0);
					}

					if (board [i] [j] == 8) {
						int[][] p1 = new int[8][];
						CopyBoard (p1, board);
							
						Rook_move (i, j, p1, 8, 1);
					}

					if (board [i] [j] == 10) {
						int[][] p1 = new int[8][];
						CopyBoard (p1, board);
							
						Queen_move (i, j, p1, 10, 1);
					}
				
					if (board [i] [j] == 12) {
						int[][] p1 = new int[8][];
						CopyBoard (p1, board);

						King_move (i, j, board, 12);
					}
				}
			}	
		}
		if (white) {
			Debug.Log (board_score (board, true));
		} else {
			Debug.Log (board_score (board, false));
		}
	}








	void set_board(){

		//rooks
		board [0] [0] = 7;
		board [7] [0] = 8;
		board [0] [7] = 7;
		board [7] [7] = 8;

		//knights
		board [0] [1] = 3;
		board [0] [6] = 3;
		board [7] [1] = 4;
		board [7] [6] = 4;

		//bishops
		board [0] [2] = 5;
		board [0] [5] = 5;
		board [7] [2] = 6;
		board [7] [5] = 6;

		//queens
		board [0] [3] = 9;
		board [7] [3] = 10;

		//kings
		board [0] [4] = 11;
		board [7] [4] = 12;

		//pawns
		for (int i = 0; i < 8; i++) {
			board [1] [i] = 1;
			board [6] [i] = 2;
		}

		draw();

	}

	void draw_board() {
		
		GameObject[] Gob = GameObject.FindGameObjectsWithTag ("piece");

		for (int i = 0; i < Gob.Length; i++) {
			Destroy (Gob [i]);
		}

		draw();
	}





	void draw() {

		for (int i = 0; i < 8; i++) {
			for (int j = 0; j < 8; j++) {


				if (board [i] [j] == 1) {
					Instantiate (white_pawn, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 2) {
					Instantiate (black_pawn, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 3) {
					Instantiate (white_knight, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 4) {
					Instantiate (black_knight, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 5) {
					Instantiate (white_bishop, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 6) {
					Instantiate (black_bishop, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 7) {
					Instantiate (white_rook, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 8) {
					Instantiate (black_rook, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 9) {
					Instantiate (white_queen, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 10) {
					Instantiate (black_queen, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 11) {
					Instantiate (white_king, new Vector3 (j, i, 0), Quaternion.identity);
				}

				if (board [i] [j] == 12) {
					Instantiate (black_king, new Vector3 (j, i, 0), Quaternion.identity);
				}
			}
		}
	}



	int[][] CopyBoard(int[][] board_copy, int[][] copy_board){

		for (int k = 0; k < 8; k++) {
			board_copy [k] = new int[8];
			for (int l = 0; l < 8; l++) {
				board_copy [k] [l] = copy_board [k] [l];
			}
		}
		return(board_copy);
	}

	void Type2_move(int i, int j, int x, int y, int[][]c_board, int piece, int side){

		bool move = true;

		int a = 0;

		while (move) {
			a++;

			if ((i + (x * a)) < 8 && (i + (x * a) > -1) && (j + (y * a)) < 8 && (j + (y * a)) > -1) {
				if (board [i + (x * a)] [j + (y * a)] == 0) {

					int[][] b = new int[8][];
					CopyBoard (b, c_board);

					b [i + (x * a)] [j + (y * a)] = piece;
					b [i] [j] = 0;
					possible_positions.Add (b);
			
				} else if (board [i + (x * a)] [j + (y * a)] % 2 == side) {

					int[][] b = new int[8][];
					CopyBoard (b, c_board);

					b [i + (x * a)] [j + (y * a)] = piece;
					b [i] [j] = 0;
					possible_positions.Add (b);

					move = false;
					a = 0;
				} else {
					move = false;
					a = 0;
				}
			} else {
				move = false;
			}
		}
	}

	void Type1_move (int i, int j, int y, int x, int[][]c_board, int piece) {

		if ((i + (x)) < 8 && (i + (x) > -1) && (j + (y)) < 8 && (j + (y)) > -1) {

			if (white) {
				if (board [i + x] [j + y] % 2 == 0) {
					int[][] b = new int[8][];
					CopyBoard (b, c_board);

					b [i + x] [j + y] = piece;
					b [i] [j] = 0;

					possible_positions.Add (b);
				}
			} else {
				if (board [i + x] [j + y] % 2 == 1 || board [i + x] [j + y] == 0) {
					int[][] b = new int[8][];
					CopyBoard (b, c_board);

					b [i + x] [j + y] = piece;
					b [i] [j] = 0;

					possible_positions.Add (b);
				}
			}
		}
	}

	void Knight_move (int i, int j, int[][] p1, int x) {

		 Type1_move(i, j, -1, 2, p1, x);
		 Type1_move(i, j, 1, 2, p1, x);
		 Type1_move(i, j, -2, 1, p1, x);
		 Type1_move(i, j, 2, 1, p1, x);
		 Type1_move(i, j, -2, -1, p1, x);
		 Type1_move(i, j, 2, -1, p1, x);
		 Type1_move(i, j, -1, -2, p1, x);
		 Type1_move(i, j, 1, -2, p1, x);
	}

	void Bishop_move (int i, int j, int[][] p1, int a, int b) {

		Type2_move (i, j, 1, 1, p1, a, b);
		Type2_move (i, j, -1, 1, p1, a, b);
		Type2_move (i, j, 1, -1, p1, a, b);
		Type2_move (i, j, -1, -1, p1, a, b);
	}

	void Rook_move (int i, int j, int[][] p1, int a, int b) {

		Type2_move (i, j, 0, 1, p1, a, b);
		Type2_move (i, j, 0, -1, p1, a, b);
		Type2_move (i, j, 1, 0, p1, a, b);
		Type2_move (i, j, -1, 0, p1, a, b);
	}
		
	void Queen_move (int i, int j, int[][] p1, int a, int b) {

		Type2_move (i, j, 0, 1, p1, a, b);
		Type2_move (i, j, 0, -1, p1, a, b);
		Type2_move (i, j, 1, 0, p1, a, b);
		Type2_move (i, j, -1, 0, p1, a, b);
		Type2_move (i, j, 1, 1, p1, a, b);
		Type2_move (i, j, -1, 1, p1, a, b);
		Type2_move (i, j, 1, -1, p1, a, b);
		Type2_move (i, j, -1, -1, p1, a, b);
	}

	void King_move (int i, int j, int[][] board, int piece) {
		
		Type1_move(i, j, 1, 0, board, piece);
		Type1_move(i, j, 0, 1, board, piece);
		Type1_move(i, j, 1, 1, board, piece);
		Type1_move(i, j, -1, 0, board, piece);
		Type1_move(i, j, 0, -1, board, piece);
		Type1_move(i, j, -1, -1, board, piece);
		Type1_move(i, j, 1, -1, board, piece);
		Type1_move(i, j, -1, 1, board, piece);
	}

		int board_score (int[][] list, bool side){

			int returnint = -1000;
			int a = 0;

			if (side) {
				a = 1;
			}

			for (int i = 0; i < 8; i++) {
				for (int j = 0; j < 8; j++) {

					if (white) {
						if (list[i] [j] == 1 + a) {
							returnint += 1;
						}
						if (board [i] [j] == 3 + a) {
							returnint += 3;
						}
						if (board [i] [j] == 5 + a) {
							returnint += 3;
						}
						if (board [i] [j] == 7 + a) {
							returnint += 5;
						}
						if (board [i] [j] == 9 + a) {
							returnint += 9;
						}
						if (board [i] [j] == 11 + a) {
							returnint += 1000;
					}
				} 
			}
		}
		return returnint;
	}
}
