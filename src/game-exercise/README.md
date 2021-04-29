# Football game exercise!

If you remember I'm from Argentina so if there is something I like is football, so here you will find a small "video game". Let's play...


# Rules

- We have 2 teams with 11 players each, all 22 players will get the possibility to score as many goals they can.
- We have a referee, who will start the match, keep the score and end the match with a winner or tied.
- We have the VAR, these guys need to check if a goal is valid and inform the referee to keep a fair score.

# To Do:

- Create the necessary exchanges, queues and bind them using an appropriate routing key.
- Publish the referee messages with the correct exchange and routing key for the players.
- Publish the players messages with the correct exchange and routing key for the VAR/referee.
- Publish the VAR messages with the correct exchange and routing key for the referee.

**... Let's Play...**