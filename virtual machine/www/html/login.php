<?php
        require "/var/www/init.php";

        $email = "";
        $password = "";
        if(isset($_POST["email"]))
        {
                $email = $_POST["email"];
        }
        if(isset($_POST["password"]))
        {
                $password = $_POST["password"];
        }

        $emailRequest = "SELECT passwd, IDuser, username FROM user WHERE email = '".$email."';";
        $results = mysqli_query($CONNECTION, $emailRequest);

        if(mysqli_num_rows($results) == 1)
        {
                while($requestLoop = mysqli_fetch_array($results))
                {
                        if($requestLoop['passwd'] == $password)
                        {
                                $finalResponse = "LS_1";
                                $finalResponse .= $RESTRICTED_MARK.$requestLoop['IDuser'];
                                $finalResponse .= $RESTRICTED_MARK.$requestLoop['username'];
                                $finalResponse .= $RESTRICTED_MARK.$email;

                                exit($finalResponse);
                        }
                }
                exit("L_1");
        }
        else if(mysqli_num_rows($results) == 0)
        {
                exit("L_2");
        }
        else
        {
                exit("L_3");
        }
?>