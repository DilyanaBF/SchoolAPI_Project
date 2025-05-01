Feature: Teacher

Teacher can create classes, add students and grades

Scenario: Teacher creates a class with subjects
  Given login with "teacher1" username and "teacher1" password
  When the teacher creates a new "DGClass3" class_name with "Math" subject_1, "Literature" subject_2, "History" subject_3
  Then validate class creation response

Scenario: Teacher adds students to a class
  Given login with "teacher1" username and "teacher1" password
  When the teacher adds a new student
  Then validate student is added

Scenario: Teacher updates grade for student 
  Given login with "teacher1" username and "teacher1" password
  When the teacher updates grade for a student per subject
  Then validate grade is updated


