### 实现功能
1. 依赖注入
2. 中英文切换
3. 基于`Velopack`的自动更新
4. Json配置
5. 基于`CommunityToolkit.Mvvm`的MVVM

### 项目效果
https://github.com/user-attachments/assets/91b19351-5975-4669-af9c-2f93ad2f38e1

### Velopack 自动更新说明
#### Windows 平台下
对发布文件进行打包后，会生成Releases文件夹

var mgr = new UpdateManager("http://localhost:4555/updates");

这里updates中放的是Relases中所有的文件，需确保 Releases 文件 能够正常访问
