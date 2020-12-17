<?php
        require "/var/www/init.php";

        $email = "";
        $password = "";
        $confPassword = "";
        $username = "";

        if(isset($_POST["email"]))
        {
                $email = $_POST["email"];
        }
        if(isset($_POST["password"]))
        {
                $password = $_POST["password"];
        }
        if(isset($_POST["confPassword"]))
        {
                $confPassword = $_POST["confPassword"];
        }
        if(isset($_POST["username"]))
        {
                $username = $_POST["username"];
        }

        //username_test
        if(strlen($username) > 20)
        {
                exit("R_U_1");
        }
        if(strpos($username, $RESTRICTED_MARK) == true)
        {
                exit("R_U_3");
        }

        //email_test
        if(!filter_var($email, FILTER_VALIDATE_EMAIL))
        {
                exit("R_E_1");
        }
        if(strlen($email) > 20)
        {
                exit("R_E_2");
        }
        if(strpos($email, $RESTRICTED_MARK) == true)
        {
                exit("R_E_4");
        }

        //password_test
        if(strlen($password) <= 7 || strlen($password) > 20)
        {
                exit("R_P_1");
        }
        if(strcspn($password, '0123456789') == strlen($password))
        {
                exit("R_P_2");
        }
        if(strpos($password, $RESTRICTED_MARK) == true)
        {
                exit("R_P_3");
        }
        if($password != $confPassword)
        {
                exit("R_P_4");
        }


        $emailRequest = "SELECT email FROM user WHERE email = '".$email."';";
        $results = mysqli_query($CONNECTION, $emailRequest);

        if(mysqli_num_rows($results) == 0)
        {
                $usernameRequest = "SELECT username FROM user WHERE username = '".$username."';";
                $results = mysqli_query($CONNECTION, $usernameRequest);

                if(mysqli_num_rows($results) == 0)
                {
                        $inputQuery = "INSERT INTO user (username, passwd, email) VALUES ('".$username."', '".$password."', '".$email."');";

                        if(mysqli_query($CONNECTION, $inputQuery))
                        {
                                exit("RS_1");
                        }
                        else
                        {
                                exit("R_DB_1");
                        }
                }
                else
                {
                        exit("R_U_2");
                }
        }
        else
        {
                exit("R_E_3");
        }
?>