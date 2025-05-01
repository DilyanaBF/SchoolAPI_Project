Feature: Login

Users log in succesfully with valid credentials else are not authenticated


Scenario Outline: Different users log in successfully with valid credentials
  Given user with role <role> logs in with valid username <username> and password <password>
  Then user is logged in successfully

Examples:
  | role      | username  | password  |
  | admin     | admin1    | admin123  |
  | teacher   | teacher1  | teacher1  |
  | moderator | moderator | moderator |
  | parent    | parent1   | parent1   |

Scenario: User cannot login with invalid credentials
  Given user logs in with invalid username <username> and password <password>
  Then the user is not logged in

  Examples:
      | username   | password  |
      | admin      | admin123  |
      | teacher1   | teacher22 |
      | moderator1 | moderator |
      | parent1    | parents   |
      |            | moderator |
      | parent1    |           |
      |            |           |



