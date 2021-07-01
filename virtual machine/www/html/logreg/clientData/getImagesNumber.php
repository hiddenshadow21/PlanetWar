<?php

$directory = getcwd()."/";
$directories = glob($directory ."*");
$nod = count($directories) - 1;

for($i = 0; $i < $nod; $i++) {
	$fileList = glob($i.'/*');
	$arr = [];
	$j = 0;

	foreach($fileList as $filename) 
	{
		if(is_file($filename)){
			$arr[$j++] = $filename;
		}
	}	
	
	if(strcmp($arr[0], $i."/description.txt") == 0 &&
		strcmp($arr[1], $i."/image.png") == 0 &&
		strcmp($arr[2], $i."/title.txt") == 0)
	{
		continue;
	}
	else
	{
		$i = 0;
		break;
	}
}	

echo($i);

?>
