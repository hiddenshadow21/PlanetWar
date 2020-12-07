<?php
        /*
         * E3 -> invalid username or password
         * E4 -> user does not exist
         * E5 -> more than one user with this email
         * S1 -> authentication successful
        */
        require "/var/www/init.php";

        $email = "";
        $password = "";
        if(isset($_POST["email"])) {
                $email = $_POST["email"];
        }
        if(isset($_POST["password"])) {
                $password = $_POST["password"];
        }

        $emailRequest = "SELECT email, passwd FROM user WHERE email = '".$email."';";
        $results = mysqli_query($CONNECTION, $emailRequest);

        if(mysqli_num_rows($results) == 1) {
                while($requestLoop = mysqli_fetch_array($results)) {
                        if($requestLoop['email'] == $email && $requestLoop['passwd'] == $password) {
                                exit("S1");
                        }
                }
                exit("E3");
        }
        else if(mysqli_num_rows($results) == 0) {
                exit("E4");
        } else {
                exit("E5");
        }
?>