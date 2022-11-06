Запускаемая команда:
```console
ab -c 10 -n 10000 http://localhost/api/v1/index.html
```

Без балансировки:
```console
This is ApacheBench, Version 2.3 <$Revision: 1901567 $>
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Licensed to The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 1000 requests
Completed 2000 requests
Completed 3000 requests
Completed 4000 requests
Completed 5000 requests
Completed 6000 requests
Completed 7000 requests
Completed 8000 requests
Completed 9000 requests
Completed 10000 requests
Finished 10000 requests


Server Software:        nginx/1.23.2
Server Hostname:        localhost
Server Port:            80

Document Path:          /api/v1/
Document Length:        0 bytes

Concurrency Level:      10
Time taken for tests:   9.890 seconds
Complete requests:      10000
Failed requests:        0
Non-2xx responses:      10000
Total transferred:      1530000 bytes
HTML transferred:       0 bytes
Requests per second:    1011.12 [#/sec] (mean)
Time per request:       9.890 [ms] (mean)
Time per request:       0.989 [ms] (mean, across all concurrent requests)
Transfer rate:          151.08 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.4      0       9
Processing:     2   10   6.1      8      90
Waiting:        2    9   6.1      8      89
Total:          3   10   6.1      8      90

Percentage of the requests served within a certain time (ms)
  50%      8
  66%     10
  75%     11
  80%     12
  90%     15
  95%     18
  98%     23
  99%     40
 100%     90 (longest request)
```

С балансировкой:
```console
This is ApacheBench, Version 2.3 <$Revision: 1901567 $>
Copyright 1996 Adam Twiss, Zeus Technology Ltd, http://www.zeustech.net/
Licensed to The Apache Software Foundation, http://www.apache.org/

Benchmarking localhost (be patient)
Completed 1000 requests
Completed 2000 requests
Completed 3000 requests
Completed 4000 requests
Completed 5000 requests
Completed 6000 requests
Completed 7000 requests
Completed 8000 requests
Completed 9000 requests
Completed 10000 requests
Finished 10000 requests


Server Software:        nginx/1.23.2
Server Hostname:        localhost
Server Port:            80

Document Path:          /api/v1/
Document Length:        0 bytes

Concurrency Level:      10
Time taken for tests:   19.033 seconds
Complete requests:      10000
Failed requests:        0
Non-2xx responses:      10000
Total transferred:      1530000 bytes
HTML transferred:       0 bytes
Requests per second:    525.39 [#/sec] (mean)
Time per request:       19.033 [ms] (mean)
Time per request:       1.903 [ms] (mean, across all concurrent requests)
Transfer rate:          78.50 [Kbytes/sec] received

Connection Times (ms)
              min  mean[+/-sd] median   max
Connect:        0    0   0.4      0       4
Processing:     4   18   8.2     17      87
Waiting:        4   18   8.1     16      86
Total:          5   19   8.2     17      87

Percentage of the requests served within a certain time (ms)
  50%     17
  66%     20
  75%     22
  80%     24
  90%     28
  95%     34
  98%     40
  99%     47
 100%     87 (longest request)
```