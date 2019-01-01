$doNotDelete = "MSSQLLocalDB", "ProjectsV13"

$instances = sqllocaldb info | Where-Object { $doNotDelete -notcontains $_ }

foreach ($instance in $instances) {
    sqllocaldb stop $instance
    sqllocaldb delete $instance
}