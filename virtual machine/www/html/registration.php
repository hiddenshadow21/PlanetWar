<?php
        /*
         * E6 -> username is too long
         * E7 -> email has syntax error
         * E8 -> email is too long
         * E9 -> password is too short or too long
         * E10 -> password has no number
         * E11 -> both passwords are not equal
         * E12 -> account with this email already exists
         * E13 -> account with this username already exists
         * E14 -> insert into database error
         * S2 -> new account created succesfully
         */
        require "/var/www/init.php";

        $email = "";
        $password = "";
        $confPassword = "";
        $username = "";

        if(isset($_POST["email"])) {
                $email = $_POST["email"];
        }
        if(isset($_POST["password"])) {
                $password = $_POST["password"];
        }
        if(isset($_POST["confPassword"])) {
                $confPassword = $_POST["confPassword"];
        }
        if(isset($_POST["username"])) {
                $username = $_POST["username"];
        }

        //username_test
        if(strlen($username) > 20) {
                exit("E6");
        }

        //email_test
        if(!filter_var($email, FILTER_VALIDATE_EMAIL)) {
                exit("E7");
        }
        if(strlen($email) > 20) {
                exit("E8");
        }

        //password_test
        if(strlen($password) <= 7 || strlen($password) > 20) {
                exit("E9");
        }
        if(strcspn($password, '0123456789') == strlen($password)) {
                exit("E10");
        }
        if($password != $confPassword) {
                exit("E11");
        }


        $emailRequest = "SELECT email FROM user WHERE email = '".$email."';";
        $results = mysqli_query($CONNECTION, $emailRequest);

        if(mysqli_num_rows($results) == 0) {
                $usernameRequest = "SELECT username FROM user WHERE username = '".$username."';";
                $results = mysqli_query($CONNECTION, $usernameRequest);

                if(mysqli_num_rows($results) == 0) {
                        $inputQuery = "INSERT INTO user (username, passwd, email) VALUES ('".$username."', '".$password."', '".$email."');";
                        if(mysqli_query($CONNECTION, $inputQuery)) {
                                exit("S2");
                        } else {
                                exit("E14");
                        }
                } else {
                        exit("E13");
                }
        } else {
                exit("E12");
        }
?>