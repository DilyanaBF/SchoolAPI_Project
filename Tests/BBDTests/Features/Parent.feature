Feature: Parent

As a parent I want to be able to check my children grades.

@tag1
Scenario: Parent can checks own child's grades
  Given parent login with "parent1" username and "parent1" password
  When a parent checks own child grades
  Then validate student grades response is successful

Scenario: Parent cannot checks other people's child grades
  Given login with "parent1" username and "parent1" password
  When a parent checks not own child grades
  Then the parent is forbidden from accessing another student's grades



