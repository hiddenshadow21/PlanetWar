<html>
<head>
	<style>
		body
		{
			background-image: url(graphics/bgLogReg.jpg);
			background-repeat: no-repeat;
			background-attachment: fixed;
			background-size: cover;
			background-color: black;
			background-position: center center;
			text-align: center;
			font-family: babaPro;
		}
		@font-face
		{
			font-family: babaPro;
			src: url(graphics/BabaPro.woff);
		}
		h1
		{
			font-size: 10.0vw;
			color: #6BECF7;
		}
		h2
		{
			font-size: 2.0vw;
			color: #64B041;
		}
		h3
		{
			font-size: 2.0vw;
			color: #D4354A;
		}
	</style>
</head>
<body>
	<h1>PlanetWar</h1>

	<?php

	require "/var/www/logreg/_newAccountVerify.php";

	$response = DownloadDataFromUrl();
	if($response == "_SUCCESSFUL")
	{
		$response = VerifyData();
		if($response == "_SUCCESSFUL")
		{
			$response = UpdateData();
			if($response == "NAV_UD_SUCCESSFUL")
			{
				echo "<h2>Your account has been activated.</br>You can now use it fully.</h2>";
				return;
			}
		}
	}

	echo "<h3>Registration error code: ".$response."</h3>";

	?>
</body>
</html>

