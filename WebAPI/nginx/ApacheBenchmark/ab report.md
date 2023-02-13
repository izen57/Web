Запускаемая команда:
```console
 .\ab -c 10 -n 10000 'http://localhost/api/v1/alarmclocks?PageNumber=0&PageSize=1'
 ```

Без балансировки:
```console
This is ApacheBench, Version 2.3 <$Revision: 1901567 $>
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Licensed to The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 500 requests
Completed 1000 requests
Completed 1500 requests
Completed 2000 requests
Completed 2500 requests
Completed 3000 requests
Completed 3500 requests
Completed 4000 requests
Completed 4500 requests
Completed 5000 requests
Finished 5000 requests


Server Software:        webapis
Server Hostname:        localhost
Server Port:            80

Document Path:          /api/v1/alarmclocks?PageNumber=0&PageSize=1
Document Length:        93 bytes

Concurrency Level:      10
Time taken for tests:   25.925 seconds
Complete requests:      5000
Failed requests:        0
Total transferred:      1260000 bytes
HTML transferred:       465000 bytes
Requests per second:    192.86 [#/sec] (mean)
Time per request:       51.850 [ms] (mean)
Time per request:       5.185 [ms] (mean, across all concurrent requests)
Transfer rate:          47.46 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.4      0       2
Processing:    27   51  16.2     48     233
Waiting:       27   51  16.2     48     233
Total:         28   52  16.2     48     233

Percentage of the requests served within a certain time (ms)
  50%     48
  66%     52
  75%     55
  80%     58
  90%     65
  95%     75
  98%    105
  99%    127
 100%    233 (longest request)
```

С балансировкой:
```console
This is ApacheBench, Version 2.3 <$Revision: 1901567 $>
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Licensed to The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 500 requests
Completed 1000 requests
Completed 1500 requests
Completed 2000 requests
Completed 2500 requests
Completed 3000 requests
Completed 3500 requests
Completed 4000 requests
Completed 4500 requests
Completed 5000 requests
Finished 5000 requests


Server Software:        webapis
Server Hostname:        localhost
Server Port:            80

Document Path:          /api/v1/alarmclocks?PageNumber=0&PageSize=1
Document Length:        93 bytes

Concurrency Level:      10
Time taken for tests:   25.627 seconds
Complete requests:      5000
Failed requests:        0
Total transferred:      1260000 bytes
HTML transferred:       465000 bytes
Requests per second:    195.11 [#/sec] (mean)
Time per request:       51.253 [ms] (mean)
Time per request:       5.125 [ms] (mean, across all concurrent requests)
Transfer rate:          48.02 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.4      0       3
Processing:    15   51  19.8     48     450
Waiting:       15   50  19.8     47     450
Total:         15   51  19.8     48     451

Percentage of the requests served within a certain time (ms)
  50%     48
  66%     51
  75%     54
  80%     56
  90%     61
  95%     68
  98%     79
  99%     91
 100%    451 (longest request)
```